using System.Collections;
using NUnit.Framework;
using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHealthTests
{
    private GameObject player;

    [UnityTest]
    public IEnumerator PlayMode_PlayerTakesDamage_HealthDecreases()
    {
        player = new GameObject("TestPlayer");
        player.SetActive(false);
        var health = player.AddComponent<Health>();
        health.maxHP = 3;
        player.SetActive(true);

        Assert.That(health.IsAlive, Is.True);
        health.Decrement();
        Assert.That(health.IsAlive, Is.True, "Один удар не должен убить игрока с 3 HP.");
        health.Decrement();
        Assert.That(health.IsAlive, Is.True, "После двух ударов должен остаться 1 HP.");
        health.Decrement();
        Assert.That(health.IsAlive, Is.False, "Третий удар должен обнулить здоровье.");
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Simulation.Clear();
        if (player != null) Object.Destroy(player);
        yield return null;
    }
}
