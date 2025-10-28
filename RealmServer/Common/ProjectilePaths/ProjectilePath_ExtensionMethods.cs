namespace Common.ProjectilePaths
{
    /// <summary>
    /// Extension methods for the <see cref="ProjectilePath"/> class.
    /// </summary>
    public static class ProjectilePath_ExtensionMethods
    {
        /// <summary>
        /// Adds a new segment to be used after the previous one.
        /// </summary>
        /// <param name="path">Base path.</param>
        /// <param name="timeMs">How long the segment lasts for.</param>
        /// <param name="segment">Segment values.</param>
        /// <returns></returns>
        public static ProjectilePath Then(this ProjectilePath path, int timeMs, ProjectilePathSegment segment)
        {
            segment.LifetimeMs = timeMs;
            path.RegisterSegment(segment);
            return path;
        }
    }
}