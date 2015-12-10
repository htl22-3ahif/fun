﻿using fun.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Basics.Skripts
{
    public sealed class RotateActorOnInputScriptElement : Element
    {
        private readonly InputElement input;
        private readonly TransformElement transform;

        public RotateActorOnInputScriptElement(Environment environment, Entity entity)
            : base(environment, entity)
        {
            if (!entity.ContainsElement<InputElement>())
                throw new NotSupportedException();

            if (!entity.ContainsElement<TransformElement>())
                throw new NotSupportedException();

            input = entity.GetElement<InputElement>() as InputElement;
            transform = entity.GetElement<TransformElement>() as TransformElement;
        }

        public override void Update(GameTime gameTime)
        {
            transform.Rotation = input.Content;//new Vector3(input.MouseDelta.Y / 100f, 0f, input.MouseDelta.X / 100f);
        }
    }
}
