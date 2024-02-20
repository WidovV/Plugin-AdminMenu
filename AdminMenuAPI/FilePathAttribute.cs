namespace AdminMenuAPI;

[AttributeUsage(AttributeTargets.Method)]
public class CategoryNameAttribute : Attribute
{
    public string CategoryName { get; }

    public CategoryNameAttribute(string categoryName)
    {
        CategoryName = categoryName;
    }
}
