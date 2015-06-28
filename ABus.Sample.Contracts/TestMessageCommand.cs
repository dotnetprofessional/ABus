namespace ABusSample.Contracts
{
    public class TestMessageCommand
    {
        public string Name { get; set; }

        public string Addresss { get; set; }
    }

    public class TestMessageCommandResponse
    {
        public string Message { get; set; }
    }
}