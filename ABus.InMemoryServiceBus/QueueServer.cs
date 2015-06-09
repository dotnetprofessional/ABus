using ABus.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABus.InMemoryServiceBus
{
    public interface IServiceClientQueue
    {
        void CreateQueue();
        void DeleteQueue();
        bool QueueExists();
    }

    public interface IServiceServerQueue
    {
    }

    class QueueSink
    {
        TransportDefinition Definition;
        public QueueSink(TransportDefinition definition)
        {
            Definition = definition;
        }

        public void Flush(QueueEndpoint endpoint, RawMessage message)
        {
            using (var client = new NamedPipeClientStream(".", Definition.Name,
                    PipeDirection.InOut, PipeOptions.None,
                    TokenImpersonationLevel.Impersonation))
            {
                client.Connect();

                NamedPipeStreamReader ss = new NamedPipeStreamReader(client);
                // Validate the server's signature string 
                if (ss.ReadString() == "I am the one true server!")
                {
                    // The client security token is sent with the first write. 
                    // Send the name of the file whose contents are returned 
                    // by the server.
                    ss.WriteString("c:\\textfile.txt");

                    // Print the file to the screen.
                    Console.Write(ss.ReadString());
                }
                else
                {
                    Console.WriteLine("Server could not be verified.");
                }
                client.Close();
            }
        }
    }

    class QueueServer : IDisposable
    {
        static int ThreadCount;

        public static void Main()
        {
            int i;
            Thread[] servers = new Thread[ThreadCount];

            for (i = 0; i < ThreadCount; i++)
            {
                servers[i] = new Thread(ServerThread);
                servers[i].Start();
            }
            Thread.Sleep(250);
            while (i > 0)
            {
                for (int j = 0; j < ThreadCount; j++)
                {
                    if (servers[j] != null)
                    {
                        if (servers[j].Join(250))
                        {
                            Console.WriteLine("Server thread[{0}] finished.", servers[j].ManagedThreadId);
                            servers[j] = null;
                            i--;    // decrement the thread watch count
                        }
                    }
                }
            }
            Console.WriteLine("\nServer threads exhausted, exiting.");
        }

        private static void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("pipe_in", PipeDirection.In, ThreadCount);

            //int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            //Console.WriteLine("Client connected on thread[{0}].", threadId);
            try
            {
                // Read the request from the client. Once the client has 
                // written to the pipe its security token will be available.

                NamedPipeStreamReader ss = new NamedPipeStreamReader(pipeServer);

                // Verify our identity to the connected client using a 
                // string that the client anticipates.

                ss.WriteString("I am the one true server!");
                string filename = ss.ReadString();

                // Read in the contents of the file while impersonating the client.
                ReadFileToStream fileReader = new ReadFileToStream(ss, filename);

                // Display the name of the user we are impersonating.
                Console.WriteLine("Reading file: {0} as user: {1}.",
                    filename, pipeServer.GetImpersonationUserName());
                pipeServer.RunAsClient(fileReader.Start);
            }
            // Catch the IOException that is raised if the pipe is broken 
            // or disconnected. 
            catch (IOException e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
            pipeServer.Close();
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }

    class NamedPipeStreamReader
    {
        Stream IoStream;
        Encoding Encoding;

        public NamedPipeStreamReader(Stream ioStream, Encoding encoding)
        {
            IoStream = ioStream;
            Encoding = encoding; // new UnicodeEncoding();
        }

        public NamedPipeStreamReader(Stream ioStream)
        {
            IoStream = ioStream;
            Encoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            return Encoding.GetString(Read());
        }

        public byte[] Read()
        {
            int length = IoStream.ReadByte();
            bool isShort = (length & 0x80) == 0x80;
            length &= ~0x80;
            if (isShort)
            {
                length <<= 8;
                length += IoStream.ReadByte();
            }
            else
            {
                length <<= 24;
                length += IoStream.ReadByte() << 16;
                length += IoStream.ReadByte() << 8;
                length += IoStream.ReadByte();
            }
            byte[] buffer = new byte[length];
            IoStream.Read(buffer, 0, length);
            return buffer;
        }
    }

    class NamedPipeStreamWriter
    {
        Stream IoStream;
        Encoding Encoding;

        public NamedPipeStreamWriter(Stream ioStream, Encoding encoding)
        {
            IoStream = ioStream;
            Encoding = encoding; // new UnicodeEncoding();
        }

        public NamedPipeStreamWriter(Stream ioStream)
        {
            IoStream = ioStream;
            Encoding = new UnicodeEncoding();
        }

        public int WriteString(string outString)
        {
            return Write(Encoding.GetBytes(outString));
        }

        public int Write(byte[] buffer)
        {
            int length = buffer.Length;
            int sentLength = length + 2;
            if (length <= (UInt16.MaxValue >> 1))
            {
                IoStream.WriteByte((byte)(((length >> 8) & 0x7F) | 1));
                IoStream.WriteByte((byte)(length & 0xFF));
            }
            else if (length <= (UInt32.MaxValue >> 1))
            {
                IoStream.WriteByte((byte)((length >> 24) & 0x7F));
                IoStream.WriteByte((byte)((length >> 16) & 0xFF));
                IoStream.WriteByte((byte)((length >> 8) & 0xFF));
                IoStream.WriteByte((byte)(length & 0xFF));
                sentLength += 2;
            }
            else
            {
                throw new ArgumentException("buffer");
            }
            IoStream.Write(buffer, 0, length);
            IoStream.Flush();

            return sentLength;
        }
    }

    // Contains the method executed in the context of the impersonated user 
    public class ReadFileToStream
    {
        private string fn;
        private NamedPipeStreamWriter ss;

        public ReadFileToStream(NamedPipeStreamWriter str, string filename)
        {
            fn = filename;
            ss = str;
        }

        public void Start()
        {
            string contents = File.ReadAllText(fn);
            ss.WriteString(contents);
        }
    }
}
