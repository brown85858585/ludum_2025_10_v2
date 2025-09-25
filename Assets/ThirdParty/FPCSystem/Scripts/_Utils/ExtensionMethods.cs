using UnityEngine;


//It is common to create a class to contain all of your extension methods. This class must be static.
public static class ExtensionMethods
{

    #region transform
    //Even though they are used like normal methods, extension methods must be declared static. 
    // Notice that the first parameter has the 'this' keyword followed by a Transform variable. 
    // This variable denotes which class the extension method becomes a part of.
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    // Find a children object by name (that contains a string)
    public static Transform FindInChildren(this Transform _trans, string _name)
    {
        foreach (Transform t in _trans.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.name.Contains(_name))
            {
                return t;
            }
        }

        return null;
    }

    // Find a children object by name (that contains a string)
    public static GameObject FindObjectInChildren(this GameObject _obj, string _name)
    {
        foreach (Transform t in _obj.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.name.Contains(_name))
            {
                return t.gameObject;
            }
        }

        return null;
    }
    #endregion

    #region string
    public static bool IsNullOrEmpty(this string value)
    {
        return (value == null || value.Length == 0);
    }

    public static bool IsNullOrWhiteSpace(this string value)
    {
        if (value == null || value.Length == 0) return true;

        for (int i = 0; i < value.Length; i++)
        {
            if (!System.Char.IsWhiteSpace(value[i])) return false;
        }

        return true;
    }
    #endregion

}