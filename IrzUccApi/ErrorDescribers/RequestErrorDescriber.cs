namespace IrzUccApi.ErrorDescribers
{
    public record RequestError
    {
        public RequestError(string code, string description)
        {
            Code = code;
            Description = description;
        }
        public string Code { get; }
        public string Description { get; }
    }

    public class RequestErrorDescriber
    {
        public static readonly RequestError AccountDeactivated = new(nameof(AccountDeactivated), "Аккаунт деактивирован!");
        public static readonly RequestError UserDoesntExist = new(nameof(UserDoesntExist), "Пользователя не существует!");
        public static readonly RequestError EmailAlreadyRegistered = new(nameof(EmailAlreadyRegistered), "Электронная почта уже используется!");
        public static readonly RequestError WrongPassword = new(nameof(WrongPassword), "Неправильный пароль!");

        public static readonly RequestError WrongRefreshToken = new(nameof(WrongRefreshToken), "Неправильный рефреш-токен!");
        public static readonly RequestError WrongJwt = new(nameof(WrongJwt), "Неправильный JWT!");

        public static readonly RequestError UserAlreadyOnPosition = new(nameof(UserAlreadyOnPosition), "Пользователь имеет должность!");
        public static readonly RequestError PositionDoesntExist = new(nameof(PositionDoesntExist), "Должности не существует!");
        public static readonly RequestError PositionAlreadyExists = new(nameof(PositionAlreadyExists), "Должность уже существует!");
        public static readonly RequestError ThereAreUsersWithThisPosition = new(nameof(ThereAreUsersWithThisPosition), "Есть пользователи с этой должностью!");
        public static readonly RequestError UserIsNotInPosition = new(nameof(UserIsNotInPosition), "Пользователь не занимает данную должность!");

        public static readonly RequestError ThereIsNoSuchRole = new(nameof(ThereIsNoSuchRole), "Роли с таким названием не существует!");
        public static readonly RequestError UserAlreadyWithThisRole = new(nameof(UserAlreadyWithThisRole), "Пользователь уже находится в данной роли!");
        public static readonly RequestError UserIsNotWithThisRole = new(nameof(UserIsNotWithThisRole), "Пользователь не имеет данной роли!");


        public static readonly RequestError EndTimeIsLessThenStartTime = new(nameof(EndTimeIsLessThenStartTime), "Время окончания не может быть меньше времени начала!");

        public static readonly RequestError MessageCantBeEmpty = new(nameof(MessageCantBeEmpty), "Сообщение не может быть пустым!");
        public static readonly RequestError ChatDoesntExist = new(nameof(ChatDoesntExist), "Чат не существует!");

        public static readonly RequestError CabinetAlreadyExists = new(nameof(CabinetAlreadyExists), "Кабинет уже существует!");
        public static readonly RequestError CabinetIsBooked = new(nameof(CabinetIsBooked), "Кабинет забронирован!");
    }
}
