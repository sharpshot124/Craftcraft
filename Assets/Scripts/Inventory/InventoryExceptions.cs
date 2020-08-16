﻿using System;
public class ItemMismatchException : Exception { public ItemMismatchException(string message) : base(message) { } public ItemMismatchException() { } }
public class StackCapacityException : Exception { public StackCapacityException(string message) : base(message) { } public StackCapacityException() { } }
public class InventoryBoundsException : Exception { public InventoryBoundsException(string message) : base(message) { } public InventoryBoundsException() { } }

public class InventoryCollisionException : Exception
{
    public Item Item;

    public InventoryCollisionException(string message, Item item) : base(message) { Item = item; }
    public InventoryCollisionException(Item item) { Item = item; }
    public InventoryCollisionException(string message) : base(message) { }
    public InventoryCollisionException() { }
}