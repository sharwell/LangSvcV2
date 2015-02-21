namespace Tvl.VisualStudio.Language.Java.Project
{
    public static class JavaProjectFileConstants
    {
        public const int UnspecifiedValue = -1;

        public const string JarReference = "JarReference";
        public const string MavenReference = "MavenReference";
        public const string IncludeInBuild = "IncludeInBuild";
        public const string SourceFolder = "SourceFolder";
        public const string TestSourceFolder = "TestSourceFolder";

        // Maven-specific metadata properties
        public const string Repository = "Repository";
        public const string GroupId = "GroupId";
        public const string ArtifactId = "ArtifactId";
        public const string Version = "Version";
        public const string Classifier = "Classifier";

        public const string JavaArchiveOutputType = "jar";
        public const string HotspotTargetVirtualMachine = "Hotspot";
        public const string JRockitTargetVirtualMachine = "JRockit";

        public const string AnyCPU = "AnyCPU";
        public const string X86 = "X86";
        public const string X64 = "X64";
    }
}
