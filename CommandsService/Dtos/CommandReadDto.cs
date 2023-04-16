namespace CommandService.Dtos
{
    public class CommandReadDto
    {
        public int Id { get; set; }
        public string HowTo { get; set; } = String.Empty;
        public string CommandLine { get; set; } = String.Empty;
        public int PlatformId { get; set; }
    }
}