using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension
{

    public abstract class Module
    {
        public abstract void Initialize();
        public abstract void Uninitialize();

        public event Action<string> OnPropertyChanged;
    }
    public class ModuleTypes : TypeSearchDefault<Module> { }


    public class AccountModule : Module
    {
        public string Name { get; set; }
        public string HeroIcon { get; set; }


        public override void Initialize()
        {
        }

        public override void Uninitialize()
        {
        }
    }
}