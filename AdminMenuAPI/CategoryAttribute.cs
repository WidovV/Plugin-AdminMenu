namespace AdminMenuAPI;

[AttributeUsage(AttributeTargets.Method)]
public class CategoryNameAttribute : Attribute
{
    public string CategoryName { get; set; } = "Other";
    public string[] CategoryFlags { get; set; } = Array.Empty<string>();

    public CategoryNameAttribute(string categoryName)
    {
        CategoryName = string.IsNullOrEmpty(categoryName) ? CategoryName : categoryName;
    }

    public CategoryNameAttribute(string categoryName, params string[] categoryFlags)
    {
        CategoryName = string.IsNullOrEmpty(categoryName) ? CategoryName : categoryName;
        CategoryFlags = categoryFlags ?? CategoryFlags;
    }
}
