using System.Collections.Generic;
using UnityEngine;

public interface IPredicateEvaluator {

    public bool? Evaluate(string predicate, string[] parametres);
}