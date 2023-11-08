using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Pokemon
{
    public int id;
    public string nom;
    public string type1;
    public string type2;
    public int evolution;
    public int evolutionId;
    public string image;
    public string description;
    public string taille;
    public string poids;
    public string talent1;
    public string talent2;
    public List<Attaque> attaques;
    public Statistique statistique;
    public List<string> lieux;
    public int tauxCapture;
    public int xp;
    public List<Loot> loot;
}

[System.Serializable]
public class Attaque
{
    public int id;
    public string nom;
    public int niveau;
}

[System.Serializable]
public class Statistique
{
    public int hp;
    public int attaque;
    public int defense;
    public int attaqueSpeciale;
    public int defenseSpeciale;
    public int vitesse;
    public int total;
}

[System.Serializable]
public class Loot
{
    public int idItem;
    public string nom;
    public int taux;
    public int[] nombre;
}

