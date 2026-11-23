using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace ProbablyHelpful
{
    public class ProbablyHelpfulTagCollection
    {
        private ProbablyHelpfulItem _helper;
        public ProbablyHelpfulTagCollection(ProbablyHelpfulItem helper)
        {
            _helper = helper;
        }
        public void Add(string tag)
        {
            _helper.GameItem.ModifyTag(tag, state => state.SetBool(true), true);
            _helper.GameItem.EnableTag(tag);
        }

        public void Set(string tag)
        {
            Add(tag);
            _helper.GameItem.SetGameItemType(tag);
        }

        public void AddMedical()
        {
            Set(TypeHelper.MEDICAL);
        }

        public void AddMedicalTool()
        {
            Set(TypeHelper.MEDICAL);
            Add(TypeHelper.MEDICAL_TOOL);
        }

        public void AddMedicalOrgan()
        {
            Set(TypeHelper.MEDICAL);
            Add(TypeHelper.ORGAN);
        }

        public void AddLuxury()
        {
            Set(TypeHelper.LUXURY_ITEM);
        }

        public void AddArt()
        {
            Set(TypeHelper.ART);
        }

        public void AddTool()
        {
            Set(TypeHelper.TOOL);
        }

        public void AddMachine()
        {
            Set(TypeHelper.MACHINE);
        }

        public void AddMaterial()
        {
            Set(TypeHelper.MATERIAL);
        }

        public void AddBasicMaterial()
        {
            AddMaterial();
            Add(TypeHelper.BASIC_MATERIAL);
        }

        public void AddRefinedMaterial()
        {
            AddMaterial();
            Add(TypeHelper.REFINED_MATERIAL);
        }

        public void AddJunk()
        {
            Set(TypeHelper.JUNK);
        }

        public void AddEnergyCell()
        {
            Set(TypeHelper.ENERGY_CELL);
        }

        public void AddModule()
        {
            Set(TypeHelper.MODULE);
        }

        public void AddNode()
        {
            Set(TypeHelper.NODE);
        }

        public void AddHydroponicModule()
        {
            AddModule();
            Add(TypeHelper.HYDROPONIC_MODULE);
        }

        public void AddAlarmModule()
        {
            AddModule();
            Add(TypeHelper.ALARM_MODULE);
        }

        public void AddPrinterModule()
        {
            AddModule();
            Add(TypeHelper.PRINTER_MODULE);
        }

        public void AddEvaporatorModule()
        {
            AddModule();
            Add(TypeHelper.EVAPORATOR_MODULE);
        }

        public void AddFilter()
        {
            Set(TypeHelper.FILTER);
        }

        public void AddWeapon()
        {
            Set(TypeHelper.WEAPON);
        }

        public void AddMeleeWeapon()
        {
            AddWeapon();
            Set(TypeHelper.MELEE_WEAPON);
        }

        public void AddFirearm()
        {
            AddWeapon();
            Set(TypeHelper.FIREARM);
        }

        public void AddModdible()
        {
            Set(TypeHelper.MODDABLE);
        }

        public void AddGunMod()
        {
            Set(TypeHelper.GUN_MOD);
        }

        public void AddAmmunition()
        {
            Set(TypeHelper.AMMUNITION);
        }

        public void AddMunition()
        {
            Set(TypeHelper.MUNITION);
        }

        public void AddArmor()
        {
            Set(TypeHelper.ARMOR);
        }
        
        public void AddRestricted1()
        {
            Set(TypeHelper.RESTRICTED_1);
        }
        
        public void AddRestricted2()
        {
            Set(TypeHelper.RESTRICTED_2);
        }
        
        public void AddRestricted3()
        {
            Set(TypeHelper.RESTRICTED_3);
        }
        
        public void AddWater()
        {
            Set(TypeHelper.WATER);
        }
        
        public void AddWaterRation()
        {
            Set(TypeHelper.WATER_RATION);
        }
        
        public void AddWaterPurificationSupply()
        {
            Set(TypeHelper.WATER_PURIFICATION_SUPPLY);
        }
        
        public void AddFood()
        {
            Set(TypeHelper.FOOD);
        }
        
        public void AddFreshFood()
        {
            AddFood();
            Add(TypeHelper.FRESH_FOOD);
        }
        
        public void AddProcessedFood()
        {
            AddFood();
            Set(TypeHelper.FRESH_FOOD);
        }
        
        public void AddTreat()
        {
            AddFood();
            Set(TypeHelper.TREAT);
        }
        
        public void AddBrevage()
        {
            AddFood();
            Set(TypeHelper.BREVAGE);
        }
        
        public void AddFarmingSupplies()
        {
            Set(TypeHelper.FARMING_SUPPLIES);
        }
        
        public void AddSeed()
        {
            Set(TypeHelper.SEED);
        }
        
        public void AddModifiedSeed()
        {
            AddSeed();
            Set(TypeHelper.MODIFIED_SEED);
        }
        
        public void AddSubstance()
        {
            Set(TypeHelper.SUBSTANCE);
        }
        
        public void AddNarcotic()
        {
            AddMedical();
            Set(TypeHelper.NARCOTIC);
        }
        
        public void AddAlcohol()
        {
            Set(TypeHelper.ALCOHOL);
        }
        
        public void AddPoison()
        {
            Set(TypeHelper.POISON);
        }
        
        public void AddDocument()
        {
            Set(TypeHelper.DOCUMENT);
        }

        public void HiddenToInspection()
        {
            Add(ContrabandHelper.ITEM_HIDDEN_TAG);
        }
    }
}