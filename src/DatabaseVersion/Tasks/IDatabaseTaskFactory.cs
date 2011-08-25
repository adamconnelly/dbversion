namespace dbversion.Tasks
{
    using System.Xml.Linq;

    using dbversion.Version;

    public interface IDatabaseTaskFactory
    {
        /// <summary>
        /// Checks whether the factory can create a task based on the specified element.
        /// </summary>
        /// <param name="element">The element containing the definition of the task.</param>
        /// <returns>true if the factory can create the task, false otherwise.</returns>
        bool CanCreate(XElement element);

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="element">The element containing the definition of the task.</param>
        /// <param name="executionOrder">The zero-based execution order of the task.</param>
        /// <param name="version">The database version that the task is contained within.</param>
        /// <returns>The new task.</returns>
        IDatabaseTask Create(XElement element, int executionOrder, IDatabaseVersion version);
    }
}
