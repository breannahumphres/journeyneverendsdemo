using System.Collections.Generic;
using System;
using UnityEngine;
public class Character
{
    public string Name {get; set;}
    public int Health {get; private set;}
    public int MaxHealth {get; private set;} = 100;
    public int Gold {get; private set;}
    public string CurrentWeapon {get; private set;}
    public List<string> Inventory {get; private set;}
    public event Action<int> OnDamageTaken;


    public Character(string name, int health, int gold, string weapon)
    {
        Name = name;
        Health = health;
        Gold = gold;
        CurrentWeapon = weapon;
        Inventory = new List<string>();
    }
    public void Equip(string weapon)
    {
        CurrentWeapon = weapon;
        Debug.Log($"{Name} equipped {weapon}.");
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
        Debug.Log($"{Name} took {amount} damage. Current health: {Health}");
        OnDamageTaken?.Invoke(amount);
        GameManager.Instance.UpdateHealthDisplay();
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"{Name} received {amount} gold. Current gold: {Gold}");
    }

    public void LoseGold(int amount)
    {
        Gold -= amount;
        if (Gold < 0) Gold = 0;
        Debug.Log($"{Name} lost {amount} gold. Current gold: {Gold}");
    }

    public void AddHealth(int amount)
    {
        Health += amount;
        if (Health > MaxHealth) Health = MaxHealth;
        Debug.Log($"{Name} gained {amount} health. Current health: {Health}");
    }
    public void AddItem(string item)
    {
        Inventory.Add(item);
        Debug.Log($"{Name} picked up {item}. Current inventory: {string.Join(", ", Inventory)}");
    }
    public void RemoveItem(string item)
    {
        if (Inventory.Contains(item))
        {
            Inventory.Remove(item);
            Debug.Log($"{Name} no longer has {item}. Current inventory: {string.Join(", ", Inventory)}");
        }
        else
        {
            Debug.Log($"{item} not found in inventory.");
        }
    }
}
