using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class PlayerStatsSerializable
{
    public float x;
    public float y;
    public bool hasBackpack;
    public bool understandsGuardians;
    public bool canReadGravestone;
    public List<string> backpack;
    public string hint;
    public int backpackIndex;
    public float speed;
}
