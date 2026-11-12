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
        public ProbablyHelpfulItem Add(string tag)
        {
            _helper.Defer(item =>
            {
                item.ModifyTag(tag, state => state.SetBool(true), true);
                item.EnableTag(tag);
            });
            return _helper;
        }

        public ProbablyHelpfulItem Set(string tag)
        {
            Add(tag);
            _helper.Defer(item => item.SetGameItemType(tag));
            return _helper;
        }

        public ProbablyHelpfulItem AddMedical()
        {
            return Set(TypeHelper.MEDICAL);
        }

        public ProbablyHelpfulItem AddMedicalTool()
        {
            Set(TypeHelper.MEDICAL);
            return Add(TypeHelper.MEDICAL_TOOL);
        }

        public ProbablyHelpfulItem AddMedicalOrgan()
        {
            Set(TypeHelper.MEDICAL);
            return Add(TypeHelper.ORGAN);
        }

        public ProbablyHelpfulItem AddLuxury()
        {
            return Set(TypeHelper.LUXURY_ITEM);
        }

        public ProbablyHelpfulItem AddArt()
        {
            return Set(TypeHelper.ART);
        }

        public ProbablyHelpfulItem AddTool()
        {
            return Set(TypeHelper.TOOL);
        }

        public ProbablyHelpfulItem AddMachine()
        {
            return Set(TypeHelper.MACHINE);
        }

        public ProbablyHelpfulItem AddMaterial()
        {
            return Set(TypeHelper.MATERIAL);
        }

        public ProbablyHelpfulItem AddBasicMaterial()
        {
            AddMaterial();
            return Add(TypeHelper.BASIC_MATERIAL);
        }

        public ProbablyHelpfulItem AddRefinedMaterial()
        {
            AddMaterial();
            return Add(TypeHelper.REFINED_MATERIAL);
        }

        public ProbablyHelpfulItem AddJunk()
        {
            return Set(TypeHelper.JUNK);
        }

        public ProbablyHelpfulItem AddEnergyCell()
        {
            return Set(TypeHelper.ENERGY_CELL);
        }

        public ProbablyHelpfulItem AddModule()
        {
            return Set(TypeHelper.MODULE);
        }

        public ProbablyHelpfulItem AddNode()
        {
            return Set(TypeHelper.NODE);
        }

        public ProbablyHelpfulItem AddHydroponicModule()
        {
            AddModule();
            return Add(TypeHelper.HYDROPONIC_MODULE);
        }

        public ProbablyHelpfulItem AddAlarmModule()
        {
            AddModule();
            return Add(TypeHelper.ALARM_MODULE);
        }

        public ProbablyHelpfulItem AddPrinterModule()
        {
            AddModule();
            return Add(TypeHelper.PRINTER_MODULE);
        }

        public ProbablyHelpfulItem AddEvaporatorModule()
        {
            AddModule();
            return Add(TypeHelper.EVAPORATOR_MODULE);
        }

        public ProbablyHelpfulItem AddFilter()
        {
            return Set(TypeHelper.FILTER);
        }

        public ProbablyHelpfulItem AddWeapon()
        {
            return Set(TypeHelper.WEAPON);
        }

        public ProbablyHelpfulItem AddMeleeWeapon()
        {
            AddWeapon();
            return Set(TypeHelper.MELEE_WEAPON);
        }

        public ProbablyHelpfulItem AddFirearm()
        {
            AddWeapon();
            return Set(TypeHelper.FIREARM);
        }

        public ProbablyHelpfulItem AddModdible()
        {
            return Set(TypeHelper.MODDABLE);
        }

        public ProbablyHelpfulItem AddGunMod()
        {
            return Set(TypeHelper.GUN_MOD);
        }

        public ProbablyHelpfulItem AddAmmunition()
        {
            return Set(TypeHelper.AMMUNITION);
        }

        public ProbablyHelpfulItem AddMunition()
        {
            return Set(TypeHelper.MUNITION);
        }

        public ProbablyHelpfulItem AddArmor()
        {
            return Set(TypeHelper.ARMOR);
        }
        
        public ProbablyHelpfulItem AddRestricted1()
        {
            return Set(TypeHelper.RESTRICTED_1);
        }
        
        public ProbablyHelpfulItem AddRestricted2()
        {
            return Set(TypeHelper.RESTRICTED_2);
        }
        
        public ProbablyHelpfulItem AddRestricted3()
        {
            return Set(TypeHelper.RESTRICTED_3);
        }
        
        public ProbablyHelpfulItem AddWater()
        {
            return Set(TypeHelper.WATER);
        }
        
        public ProbablyHelpfulItem AddWaterRation()
        {
            return Set(TypeHelper.WATER_RATION);
        }
        
        public ProbablyHelpfulItem AddWaterPurificationSupply()
        {
            return Set(TypeHelper.WATER_PURIFICATION_SUPPLY);
        }
        
        public ProbablyHelpfulItem AddFood()
        {
            return Set(TypeHelper.FOOD);
        }
        
        public ProbablyHelpfulItem AddFreshFood()
        {
            AddFood();
            return Add(TypeHelper.FRESH_FOOD);
        }
        
        public ProbablyHelpfulItem AddProcessedFood()
        {
            AddFood();
            return Set(TypeHelper.FRESH_FOOD);
        }
        
        public ProbablyHelpfulItem AddTreat()
        {
            AddFood();
            return Set(TypeHelper.TREAT);
        }
        
        public ProbablyHelpfulItem AddBrevage()
        {
            AddFood();
            return Set(TypeHelper.BREVAGE);
        }
        
        public ProbablyHelpfulItem AddFarmingSupplies()
        {
            return Set(TypeHelper.FARMING_SUPPLIES);
        }
        
        public ProbablyHelpfulItem AddSeed()
        {
            return Set(TypeHelper.SEED);
        }
        
        public ProbablyHelpfulItem AddModifiedSeed()
        {
            AddSeed();
            return Set(TypeHelper.MODIFIED_SEED);
        }
        
        public ProbablyHelpfulItem AddSubstance()
        {
            return Set(TypeHelper.SUBSTANCE);
        }
        
        public ProbablyHelpfulItem AddNarcotic()
        {
            AddMedical();
            return Set(TypeHelper.NARCOTIC);
        }
        
        public ProbablyHelpfulItem AddAlcohol()
        {
            return Set(TypeHelper.ALCOHOL);
        }
        
        public ProbablyHelpfulItem AddPoison()
        {
            return Set(TypeHelper.POISON);
        }
        
        public ProbablyHelpfulItem AddDocument()
        {
            return Set(TypeHelper.DOCUMENT);
        }

        public ProbablyHelpfulItem HiddenToInspection()
        {
            return Add(ContrabandHelper.ITEM_HIDDEN_TAG);
        }
    }
}