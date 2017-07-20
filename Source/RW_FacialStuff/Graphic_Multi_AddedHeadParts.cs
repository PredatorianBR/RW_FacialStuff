﻿using UnityEngine;
using Verse;

namespace FacialStuff
{
    using System;

    public class Graphic_Multi_AddedHeadParts : Graphic
    {
        private Material[] mats = new Material[4];

        public string GraphicPath
        {
            get
            {
                return this.path;
            }
        }

        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            switch (rot.AsInt)
            {
                case 0:
                    return this.MatBack;
                case 1:
                    return this.MatRight;
                case 2:
                    return this.MatFront;
                case 3:
                    return this.MatLeft;
                default:
                    return BaseContent.BadMat;
            }
        }

        public  Material MatLeft
        {
            get
            {
                return this.mats[3];
            }
        }

        public override Material MatFront
        {
            get
            {
                return this.mats[2];
            }
        }

        public  Material MatRight
        {
            get
            {
                return this.mats[1];
            }
        }

        public override Material MatBack
        {
            get
            {
                return this.mats[0];
            }
        }

        public override bool ShouldDrawRotated
        {
            get
            {
                return this.MatRight == this.MatBack;
            }
        }



        public override void Init(GraphicRequest req)
        {
            this.data = req.graphicData;
            this.path = req.path;
            this.color = req.color;
            this.colorTwo = req.colorTwo;
            this.drawSize = req.drawSize;
            Texture2D[] array = new Texture2D[4];

            string addedpartName = null;
            string side = null;
            string crowntype = null;

            string fileNameWithoutExtension = req.path;
            string[] array2 = fileNameWithoutExtension.Split('_');
            try
            {
                addedpartName = array2[0];
                side = array2[1];
                crowntype = array2[2];
            }
            catch (Exception ex)
            {
                Log.Error("Parse error with head graphic at " + req.path + ": " + ex.Message);
            }

            if (ContentFinder<Texture2D>.Get(req.path + "_front", false))
            {
                array[2] = ContentFinder<Texture2D>.Get(req.path + "_front");
            }
            else
            {
                Log.Message(
                    "Facial Stuff: Failed to get front texture at " + req.path + "_front"
                    + " - Graphic_Multi_AddedHeadParts");
                return;

                // array[2] = MaskTextures.BlankTexture();
            }

            if (ContentFinder<Texture2D>.Get(addedpartName + "_" + crowntype + "_side"))
            {
                if (side.Equals("Right"))
                {
                    if (ContentFinder<Texture2D>.Get(addedpartName + "_" + crowntype + "_side2", false))
                    {
                        array[3] = ContentFinder<Texture2D>.Get(addedpartName + "_" + crowntype + "_side2");

                    }
                    else
                    {
                        array[3] = MaskTextures.BlankTexture();
                    }
                }
                else
                {
                    array[3] = ContentFinder<Texture2D>.Get(addedpartName + "_" + crowntype + "_side");
                }

                if (side.Equals("Left"))
                {
                    if (ContentFinder<Texture2D>.Get(addedpartName + "_" + crowntype + "_side2", false))
                    {
                        array[1] = ContentFinder<Texture2D>.Get(addedpartName + "_" + crowntype + "_side2");
                    }
                    else
                    {
                        array[1] = MaskTextures.BlankTexture();
                    }
                }
                else
                {
                    array[1] = ContentFinder<Texture2D>.Get(addedpartName + "_" + crowntype + "_side");
                }
            }
            else
            {
                Log.Message("Facial Stuff: No texture found at " + addedpartName + "_" + crowntype + "_side" + " - Graphic_Multi_AddedHeadParts");
                array[3] = MaskTextures.BlankTexture();
            }


            if (ContentFinder<Texture2D>.Get(req.path + "_back", false))
            {
                array[0] = ContentFinder<Texture2D>.Get(req.path + "_back");
            }
            else
            {
                array[0] = MaskTextures.BlankTexture();
            }

            for (int i = 0; i < 4; i++)
            {
                if (array[i] == null)
                {
                    Log.Message("Array = null at: " + i);
                }

                MaterialRequest req2 = default(MaterialRequest);
                req2.mainTex = array[i];
                req2.shader = req.shader;
                req2.color = this.color;
                req2.colorTwo = this.colorTwo;

                // req2.maskTex = array2[i];
                this.mats[i] = MaterialPool.MatFrom(req2);
            }
        }

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return GraphicDatabase.Get<Graphic_Multi>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data);
        }

        public override string ToString()
        {
            return string.Concat("Multi(initPath=", this.path, ", color=", this.color, ", colorTwo=", this.colorTwo, ")");
        }

        public override int GetHashCode()
        {
            int seed = 0;
            seed = Gen.HashCombine(seed, this.path);
            seed = Gen.HashCombineStruct(seed, this.color);
            return Gen.HashCombineStruct(seed, this.colorTwo);
        }
    }

}