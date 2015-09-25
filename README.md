[![NuGet version](https://badge.fury.io/nu/ObjectDiffer.svg)](http://badge.fury.io/nu/ObjectDiffer)
# ObjectDiffer
A class library for performing a diff of any 2 objects of the same type.

You need to use an instance of `IDiffer` to perform the diff. The easiest way to get an instance of `IDiffer` is by using the `DifferFactory`:
```csharp
var factory = new DifferFactory();
var differ = factory.GetDefault();
```

If you are using ninject, you can also load the `ObjectDifferModule` module:
```csharp
var kernel = new StandardKernel(new ObjectDifferModule());
// you can now inject an IDiffer into your classes, or get an instance directly:
var differ = kernel.Get<IDiffer>();
```

`IDiffer` has only one method, `Diff<T>(T newObj, T oldObj)`. This returns a `Difference` object, or `null` if there is no difference. `Difference` has a property `ChildDiffs` which is a list of `Difference` objects representing the changes to child properties. It also contains the `PropertyName` ("self" for the top level object, and "Item" for items in an array), the `NewValue` and the `OldValue`. 

## Notes

1. When the differ is diffing an array, it needs to determine which elements represent the same object in each list. For example:
```csharp
class MyObj
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public void DiffAList(){
  var differ = kernel.Get<IDiffer>();
  
  var obj1 = new MyObj{Id = 1, Name = "object"};
  var obj2 = new MyObj{Id = 1337, Name = "another object"};

  var oldList = new List<MyObj>{obj1, obj2};
  var newList = new List<MyObj>{obj2, obj1}; //note that they are added in reverse order

  var diff = differ.Diff(newList, oldList);
  // we would expect the diff to be null, since the elements in the list are the same
  // however, if the differ just compares the objects at each index of the array, it would think we have changed both elements
}
```
To solve this problem, the differ assumes that if the hash code of the objects are equal, as determined by the `Object.Equals` method, they are supposed to be the same object. So in the example above, we would just add a `Equals` method to `MyObj`:
```csharp
class MyObj
{
    public int Id { get; set; }
    public string Name { get; set; }
    
	public bool Equals(object x, object y)
	{
		return ((MyObj) x).Id == ((MyObj) y).Id;
	}
}
```
If you would like to employ a different strategy for determining equality, you can override the equality comparer that the differ uses like so:
```csharp
// you have an IEqualityComparer called comparer that you would like to use
kernel.Rebind<IEqualityComparer<object>>().ToConstant(comparer).Named("SameObjectComparer");
```
