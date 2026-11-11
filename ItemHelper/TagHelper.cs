using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace ProbablyHelpful
{
    public partial class ProbablyHelpfulItem
    {
        public ProbablyHelpfulItem AddTag(string tag)
        {
            _item.ModifyTag(tag, state => state.SetBool(true), true);
            _item.EnableTag(tag);
            return this;
        }

        public ProbablyHelpfulItem SetTag(string tag)
        {
            AddTag(tag);
            _item.SetGameItemType(tag);
            return this;
        }

        public ProbablyHelpfulItem AddTagMedical()
        {
            SetTag(TypeHelper.MEDICAL);
            return this;
        }

        public ProbablyHelpfulItem AddTagMedicalTool()
        {
            SetTag(TypeHelper.MEDICAL);
            AddTag(TypeHelper.MEDICAL_TOOL);
            return this;
        }

        public ProbablyHelpfulItem AddTagMedicalOrgan()
        {
            SetTag(TypeHelper.MEDICAL);
            AddTag(TypeHelper.ORGAN);
            return this;
        }

        public ProbablyHelpfulItem AddTagLuxury()
        {
            SetTag(TypeHelper.LUXURY_ITEM);
            return this;
        }

        public ProbablyHelpfulItem AddTagArt()
        {
            SetTag(TypeHelper.ART);
            return this;
        }

        public ProbablyHelpfulItem AddTagTool()
        {
            SetTag(TypeHelper.TOOL);
            return this;
        }

        public ProbablyHelpfulItem AddTagMachine()
        {
            SetTag(TypeHelper.MACHINE);
            return this;
        }

        public ProbablyHelpfulItem AddTagMaterial()
        {
            SetTag(TypeHelper.MATERIAL);
            return this;
        }

        public ProbablyHelpfulItem AddTagBasicMaterial()
        {
            AddTagMaterial();
            AddTag(TypeHelper.BASIC_MATERIAL);
            return this;
        }

        public ProbablyHelpfulItem AddTagRefinedMaterial()
        {
            AddTagMaterial();
            AddTag(TypeHelper.REFINED_MATERIAL);
            return this;
        }

        public ProbablyHelpfulItem AddTagJunk()
        {
            SetTag(TypeHelper.JUNK);
            return this;
        }

        public ProbablyHelpfulItem AddTagEnergyCell()
        {
            SetTag(TypeHelper.ENERGY_CELL);
            return this;
        }

        public ProbablyHelpfulItem AddTagModule()
        {
            SetTag(TypeHelper.MODULE);
            return this;
        }

        public ProbablyHelpfulItem AddTagNode()
        {
            SetTag(TypeHelper.NODE);
            return this;
        }

        public ProbablyHelpfulItem AddTagHydroponicModule()
        {
            AddTagModule();
            AddTag(TypeHelper.HYDROPONIC_MODULE);
            return this;
        }

        public ProbablyHelpfulItem AddTagAlarmModule()
        {
            AddTagModule();
            AddTag(TypeHelper.ALARM_MODULE);
            return this;
        }

        public ProbablyHelpfulItem AddTagPrinterModule()
        {
            AddTagModule();
            AddTag(TypeHelper.PRINTER_MODULE);
            return this;
        }

        public ProbablyHelpfulItem AddTagEvaporatorModule()
        {
            AddTagModule();
            AddTag(TypeHelper.EVAPORATOR_MODULE);
            return this;
        }

        public ProbablyHelpfulItem AddTagFilter()
        {
            SetTag(TypeHelper.FILTER);
            return this;
        }

        public ProbablyHelpfulItem AddTagWeapon()
        {
            SetTag(TypeHelper.WEAPON);
            return this;
        }

        public ProbablyHelpfulItem AddTagMeleeWeapon()
        {
            AddTagWeapon();
            SetTag(TypeHelper.MELEE_WEAPON);
            return this;
        }

        public ProbablyHelpfulItem AddTagFirearm()
        {
            AddTagWeapon();
            SetTag(TypeHelper.FIREARM);
            return this;
        }

        public ProbablyHelpfulItem AddTagModible()
        {
            SetTag(TypeHelper.MODDABLE);
            return this;
        }

        
    }
}