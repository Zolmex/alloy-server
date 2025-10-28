package com.company.assembleegameclient.objects.particles
{
import com.company.assembleegameclient.objects.thrown.*;
   import com.company.assembleegameclient.objects.particles.ParticleEffect;
   import flash.geom.Point;
   
   public class SheatheSlashEffect extends ParticleEffect
   {

      public var color_:uint;
      public var numParts_:int;

      public function SheatheSlashEffect(color:uint, size:int, numParts:int)
      {
         super();
         this.color_ = color;
         size_ = size;
         this.numParts_ = numParts;
      }
      
      override public function update(time:int, dt:int) : Boolean
      {
         var color:uint = 0;
         var part:Particle = null;
         var startX:Number = x_ - Math.random() * 0.5;
         var startY:Number = y_ - Math.random() * 0.5;
         var endX:Number = x_ + Math.random() * 0.5;
         var endY:Number = y_ + Math.random() * 0.5;
         var m:Number = (endY - startY) / (endX - startX);
         var b:Number = startY - m * startX;
         for(var i:int = 0; i < this.numParts_; i++)
         {
            color = this.color_;
            part = new SlashParticle(color,0.5,size_,300 + Math.random() * 100,Math.random() * -0.5,Math.random() * -0.5,0);

            var x:Number = startX + i * (0.5 / this.numParts_);
            var y:Number = m * x + b;
            map_.addObj(part, x, y);
         }
         return false;
      }
   }
}

import com.company.assembleegameclient.objects.particles.Particle;
import flash.geom.Vector3D;

class SlashParticle extends Particle
{


   public var lifetime_:int;

   public var timeLeft_:int;

   protected var moveVec_:Vector3D;

   function SlashParticle(color:uint, z:Number, size:int, lifetime:int, moveVecX:Number, moveVecY:Number, moveVecZ:Number)
   {
      this.moveVec_ = new Vector3D();
      super(color,z,size);
      this.timeLeft_ = this.lifetime_ = lifetime;
      this.moveVec_.x = moveVecX;
      this.moveVec_.y = moveVecY;
      this.moveVec_.z = moveVecZ;
   }

   override public function update(time:int, dt:int) : Boolean
   {
      this.timeLeft_ = this.timeLeft_ - dt;
      if(this.timeLeft_ <= 0)
      {
         return false;
      }
      x_ = x_ + this.moveVec_.x * dt * 0.002;
      y_ = y_ + this.moveVec_.y * dt * 0.002;
      z_ = z_ + this.moveVec_.z * dt * 0.002;
      return true;
   }
}