using System.Collections;
using System.Collections.Generic;
public class CapturePoint {
    public string TextureName { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public List<Neighbour> Neighbours { get; set; }
}