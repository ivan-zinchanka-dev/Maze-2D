namespace JanZinch.Services.Logging.Contracts.Generic
{
    public interface ILogger<out TCategory> : ILogger { }
}