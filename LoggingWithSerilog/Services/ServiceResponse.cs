namespace LoggingWithSerilog.Services;

public class ServiceResponse<T>
{
    public T Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public int StatusCode { get; set; } = 200;
}