using NuGet.Protocol.Plugins;

namespace IrzUccApi.Enums
{
    static public class RequestErrorMessages
    {
        static public readonly string UserDoesntExistsMessage = "User does not exist!";
        static public readonly string UserAlreadyExistsMessage = "User already exists!";

        static public readonly string PositionDoesntExistsMessage = "Position does not exists!";
        static public readonly string PositionAlreadyExistsMessage = "Position already exists!";
        static public readonly string ThereAreUsersWithThisPositionMessage = "There are users with this position!";
    }
}