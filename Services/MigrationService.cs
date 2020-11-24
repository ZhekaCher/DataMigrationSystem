using System.Threading.Tasks;
using NLog;

namespace DataMigrationSystem.Services
{
    /// @author Yevgeniy Cherdantsev
    /// @date 18.02.2020 10:41:48
    /// @version 1.0
    /// <summary>
    /// Абстрактный класс для построения миграций на его основе
    /// </summary>
    public abstract class MigrationService
    {
        protected int NumOfThreads;

        protected MigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
            // ReSharper disable once VirtualMemberCallInConstructor
            Logger = LogManager.GetLogger(GetType().Name, GetType());
        }

        /// <summary>
        /// Логгер текущего класса
        /// </summary>
        protected Logger Logger { get; }


        /// @author Yevgeniy Cherdantsev
        /// @date 18.02.2020 10:43:57
        /// @version 1.0
        /// <summary>
        /// Метод запускающий процесс миграции
        /// </summary>
        /// <returns>Task</returns>
        public abstract Task StartMigratingAsync();
    }
}