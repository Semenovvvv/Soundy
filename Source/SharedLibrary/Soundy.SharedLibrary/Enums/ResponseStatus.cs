namespace Soundy.SharedLibrary.Enums
{
    /// <summary>
    /// Статусы ответов 
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary> Успешное выполнение </summary> 
        Success,
        /// <summary> Запись не найдена </summary> 
        NotFound,
        /// <summary> Некорректные входные данные </summary> 
        InvalidInput,
        /// <summary> Ошибка авторизации </summary> 
        Unauthorized,
        /// <summary> Конфликт (например, дубликат) </summary> 
        Conflict,
        /// <summary> Ошибка сервера (исключение) </summary> 
        InternalError,
        /// <summary> Ошибка внешних сервисов (исключение) </summary> 
        ExternalError
    }
}
