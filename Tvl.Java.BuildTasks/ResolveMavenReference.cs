namespace Tvl.Java.BuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Directory = System.IO.Directory;
    using File = System.IO.File;
    using Path = System.IO.Path;

    public class ResolveMavenReference : Task, ICancelableTask
    {
        private readonly List<ITaskItem> _resolvedFiles = new List<ITaskItem>();

        [Required]
        public ITaskItem[] References
        {
            get;
            set;
        }

        [Required]
        public ITaskItem[] Repositories
        {
            get;
            set;
        }

        [Required]
        public string PackageFolder
        {
            get;
            set;
        }

        [Output]
        public ITaskItem[] ResolvedFiles
        {
            get
            {
                return _resolvedFiles.ToArray();
            }
        }

        public ResolveMavenReference()
        {
        }

        public override bool Execute()
        {
            bool success = true;

            List<Uri> repositories = GetRepositories();

            HashSet<string> resolvedReferences = new HashSet<string>();
            foreach (ITaskItem reference in References)
            {
                success &= ResolveReference(reference, repositories, resolvedReferences);
            }

            return success;
        }

        private List<Uri> GetRepositories()
        {
            List<Uri> result = new List<Uri>();
            if (Repositories != null)
            {
                foreach (ITaskItem repository in Repositories)
                {
                    result.Add(new Uri(repository.ItemSpec));
                }
            }

            if (Repositories.Length == 0)
            {
                // Add Maven central
                result.Add(new Uri("https://repo1.maven.org/maven/"));
            }

            return result;
        }

        private bool ResolveReference(ITaskItem reference, IEnumerable<Uri> repositories, ISet<string> resolvedReferences)
        {
            string groupId = reference.GetMetadata("GroupId");
            string artifactId = reference.GetMetadata("ArtifactId");
            string version = reference.GetMetadata("Version");
            string classifier = reference.GetMetadata("Classifier");
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(artifactId) || string.IsNullOrEmpty(version))
                return false;

            // {PackageFolder}/{groupId}.{artifactId}.{version}/{artifactId}-{version}.jar
            string artifactFolder = string.Format("{0}.{1}.{2}", groupId, artifactId, version);
            if (resolvedReferences.Contains(artifactFolder))
                return true;

            string classifierSuffix = string.IsNullOrEmpty(classifier) ? string.Empty : ("-" + classifier);
            string artifactFile = string.Format("{0}-{1}{2}.jar", artifactId, version, classifierSuffix);
            string artifactPath = Path.Combine(PackageFolder, artifactFolder, artifactFile);
            if (!File.Exists(artifactPath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(artifactPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(artifactPath));

                foreach (Uri repository in repositories)
                {
                    if (TryDownloadArtifact(artifactPath, repository, groupId, artifactId, version, classifier))
                        break;
                }

                if (!File.Exists(artifactPath))
                {
                    string classifierError = string.IsNullOrEmpty(classifier) ? string.Empty : (":" + classifier);
                    Log.LogError("Unable to resolve Maven artifact '{0}:{1}:{2}{3}'", groupId, artifactId, version, classifierError);
                    return false;
                }
            }

            TaskItem resolved = new TaskItem(artifactPath);
            _resolvedFiles.Add(resolved);
            resolvedReferences.Add(artifactFolder);

            // TODO: resolve transitive dependencies

            return true;
        }

        private bool TryDownloadArtifact(string artifactPath, Uri repository, string groupId, string artifactId, string version, string classifier)
        {
            string classifierSuffix = string.IsNullOrEmpty(classifier) ? string.Empty : ("-" + classifier);
            Uri fullUri = new Uri(repository, string.Format("{0}/{1}/{2}/{1}-{2}{3}.jar", groupId.Replace('.', '/'), artifactId, version, classifierSuffix));
            WebClient webClient = new WebClient();
            try
            {
                webClient.DownloadFile(fullUri, artifactPath);
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        public void Cancel()
        {
        }
    }
}
