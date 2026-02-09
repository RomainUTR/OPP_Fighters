using TMPro;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public SO_FighterData data;

    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text CritText;
    [SerializeField] private UnityEngine.UI.Slider healthBar;

    private string _name;
    private int _currentHealth;
    private int _critChance;

    private void Start()
    {
        _name = data.FighterName;
        _currentHealth = data.MaxHealth;
        _critChance = data.CriticalChance;

        if (healthBar != null)
        {
            healthBar.maxValue = data.MaxHealth;
            healthBar.value = _currentHealth;
        }

        if (NameText != null)
        {
            NameText.text = _name;
        }
        if (CritText != null)
        {
            CritText.text = "Crit: " + _critChance + "%";
        }
    }

    public void Attack(Fighter target)
    {
        int damage = 1;

        int randomValue = Random.Range(0, 101); 
        if (randomValue < _critChance)
        {
            damage *= 2; // Dégâts doublés
            Debug.Log(_name + " inflige un COUP CRITIQUE !");
        }
        target.TakeDamage(damage);
        
        Debug.Log(_name + " attaque " + target.GetName() + " et inflige " + damage + " dégâts.");
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        if (healthBar != null)
        {
            healthBar.value = _currentHealth; 
        }
    }

    public string GetName()
    {
        return _name;
    }

    public bool IsAlive()
    {
        return _currentHealth > 0;
    }
}