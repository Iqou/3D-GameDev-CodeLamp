using UnityEngine;

public interface IObjectiveFocus
{
    Transform FocusPoint { get; } 
    string HintText { get; }     
    int SpotlightOrder { get; } 
}