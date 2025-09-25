// http://www.jianshu.com/p/4047088b6861
// http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/

using System.Collections.Generic;

public class ReorderableListBase { }

public class ReorderableList<T> : ReorderableListBase
{
	public List<T> List;
}