namespace IrzUccApi.Enums
{
    public static class RequestErrorMessages
    {
        public static readonly string AccountDeactivated = "Аккаунт деактивирован!";
        public static readonly string UserDoesntExistMessage = "Пользователя не существует!";
        public static readonly string EmailAlreadyRegistered = "Электронная почта уже используется!";
        public static readonly string WrongPassword = "Неправильный пароль!";

        public static readonly string WrongRefreshToken = "Неправильный рефреш-токен!";
        public static readonly string WrongJwt = "Неправильный JWT!";

        public static readonly string UserAlreadyOnPosition = "Пользователь имеет должность!";
        public static readonly string PositionDoesntExistMessage = "Должности не существует!";
        public static readonly string PositionAlreadyExistsMessage = "Должность уже существует!";
        public static readonly string ThereAreUsersWithThisPositionMessage = "Есть пользователи с этой должностью!";
        public static readonly string UserIsNotInPosition = "Пользователь не занимает данную должность!";

        public static readonly string EndTimeIsLessThenStartTime = "Время окончания не может быть меньше времени начала!";

        public static readonly string MessageCantBeEmpty = "Сообщение не может быть пустым!";
        public static readonly string ChatDoesntExist = "Чат не существует!";

        public static readonly string CabinetAlreadyExists = "Кабинет уже существует!";
        public static readonly string CabinetIsBooked = "Кабинет забронирован!";
    }
}