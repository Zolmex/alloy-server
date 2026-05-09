namespace Common.Projectiles.ProjectilePaths;

public static class ProjectilePath_ExtensionMethods {
    public static ProjectilePath Then(this ProjectilePath path, int timeMs, ProjectilePathSegment segment) {
        segment.LifetimeMs = timeMs;
        path.RegisterSegment(segment);
        return path;
    }
}