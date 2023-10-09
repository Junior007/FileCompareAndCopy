namespace FileCompareAndCopy
{
    internal class MetaData
    {
        public Type Type { get; }

        public Object Value { get; }
        public string Name { get; }

        public MetaData(Object value, string name, Type type)
        {
            Value = value;
            Type = type;
            Name = name;
        }
        public T GetValueAs<T>()
        {
            if (Type == typeof(T))
            {
                return (T)Value;
            }
            throw new InvalidCastException($"No se puede convertir el valor a {typeof(T).Name}");
        }
    }
}
