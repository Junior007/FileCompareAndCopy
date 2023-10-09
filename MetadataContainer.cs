
namespace FileCompareAndCopy
{
    internal class MetadataContainer
    {
        private List<MetaData> metadatas;

        public MetadataContainer()
        {
            metadatas = new List<MetaData>();
        }

        public bool HasMetadata => metadatas.Count > 0;

        public void AddMetadata(MetaData metadata)
        {
            metadatas.Add(metadata);
        }

        public T GetMetadataValue<T>(string name)
        {
            MetaData metadata = metadatas.FirstOrDefault(m => m.Name == name);

            if (metadata != null)
            {
                return metadata.GetValueAs<T>();
            }
            else
            {
                throw new KeyNotFoundException($"No se encontró un metadato con el nombre '{name}'.");
            }
        }
    }

}
