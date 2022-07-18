
using System.Reflection;

namespace ConfigMerger;
public class AttributePropertyInfo<T> where T : Attribute
{
    public AttributePropertyInfo(PropertyInfo prop, T attr)
    {
        PropInfo = prop;
        AttributeInfo = attr;
    }
    public PropertyInfo PropInfo { get; set; }
    public T AttributeInfo { get; set; }

    public bool Set { get; set; }

}


