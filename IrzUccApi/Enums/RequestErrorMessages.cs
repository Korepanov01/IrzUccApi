namespace IrzUccApi.Enums
{
    static public class RequestErrorMessages
    {
        static public readonly string AccountDeactivated = "Account is deactivated!";
        static public readonly string UserDoesntExistMessage = "User does not exist!";
        static public readonly string EmailAlreadyRegistered = "Email already used!";
        static public readonly string WrongPassword = "Wrong password!";

        static public readonly string WrongRefreshToken = "Wrong refresh token!";
        static public readonly string WrongJwt = "Wrong JWT!";

        static public readonly string UserAlreadyOnPosition = "User already on position!";
        static public readonly string PositionDoesntExistMessage = "Position does not exist!";
        static public readonly string PositionAlreadyExistsMessage = "Position already exists!";
        static public readonly string ThereAreUsersWithThisPositionMessage = "There are users with this position!";
        static public readonly string UserIsNotInPosition = "User is not in position!";

        static public readonly string EndTimeIsLessThenStartTime = "End time is less then start time!";

        static public readonly string MessageCantBeEmpty = "Message can't be empty!";
        static public readonly string ChatDoesntExist = "Chat doesn't exist!";

        static public readonly string CabinetAlreadyExists = "Cabinet already exists!";
        static public readonly string CabinetIsBooked = "Cabinet is booked!";
    }
}