namespace ABus.Contracts
{ 
    public class Transport
    {
        /// <summary>
        /// A friendly name that can be used to reference the transport
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A fully qualified definition of the transport class to use
        /// </summary>
        public string TransportType { get; set; }
    }
}
