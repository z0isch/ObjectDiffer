# ObjectDiffer
A class library for performing a diff of any 2 objects of the same type.

The easiest way to use ObjectDiffer is by loading the `ObjectDifferModule` ninject module:
```csharp
var kernel = new StandardKernel(new ObjectDifferModule());
// you can now inject an IDiffer into your classes, or get an instance directly:
var differ = kernel.Get<IDiffer>();
```

`IDiffer` has only one method, `Diff<T>(T newObj, T oldObj)`. This returns a `Difference` object, or `null` if there is no difference. `Difference` has a property `ChildDiffs` which is a list of `Difference` objects representing the changes to child properties. It also contains the `PropertyName` ("self" for the top level object, and "Item" for items in an array), the `NewValue` and the `OldValue`. 

## Notes

1. The differ determines if 2 objects are equal using the `Equals` method, rather than reference equality.

2. When the differ is diffing an array, it needs to determine which elements represent the same object in each list. For example:
```csharp
class MyObj
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public void DiffAList(){
  var differ = kernel.Get<IDiffer>();
  
  var obj = new MyObj{ Id = 1, Name = "object"};
  var oldList = new List<MyObj>{obj1, new MyObj{Id = 1337, Name = "another object"}};
  // clone the old list
  var newList = oldList.Select(x => x).ToList();
  // move obj from the start of the old list to the end of the new one
  newList.Remove(obj);
  newList.Add(obj);

  var diff = differ.Diff(newList, oldList);
  // we would expect the diff to be null, since the elements in the list are the same
  // however, if the differ just compares the objects at each index of the array, it would think we have changed both elements
}
```
To solve this problem, the differ assumes that if the hash code of the objects are equal, they are supposed to be the same object. So in the example above, we would just add a `GetHashCode()` method to `MyObj`:
```csharp
class MyObj
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public override int GetHashCode()
    {
        return Id;
    }
}
```
If you would like to employ a different strategy for determining equality, you can override the equality comparer that the differ uses like so:
```csharp
// you have an IEqualityComparer called comparer that you would like to use
kernel.Rebind<IEqualityComparer<object>>().ToConstant(comparer).Named("SameObjectComparer");
```
