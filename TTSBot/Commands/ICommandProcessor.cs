namespace TTSBot.Commands;

public interface ICommandProcessor
{
    static abstract Delegate ProcessCommand { get; }   
}