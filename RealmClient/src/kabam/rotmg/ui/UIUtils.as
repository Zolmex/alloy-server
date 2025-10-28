package kabam.rotmg.ui
{
import flash.display.DisplayObject;
import flash.display.Sprite;
import flash.display.StageQuality;
import flash.geom.Point;
import flash.geom.Rectangle;

public class UIUtils
   {
      
      private static const NOTIFICATION_BACKGROUND_WIDTH:Number = 95;
      
      private static const NOTIFICATION_BACKGROUND_HEIGHT:Number = 25;
      
      private static const NOTIFICATION_BACKGROUND_ALPHA:Number = 0.4;
      
      private static const NOTIFICATION_BACKGROUND_COLOR:Number = 0;
      
      public static const NOTIFICATION_SPACE:uint = 28;
       
      
      public function UIUtils()
      {
         super();
      }
      
      public static function returnHudNotificationBackground() : Sprite
      {
         var background:Sprite = new Sprite();
         background.graphics.beginFill(NOTIFICATION_BACKGROUND_COLOR,NOTIFICATION_BACKGROUND_ALPHA);
         background.graphics.drawRoundRect(0,0,NOTIFICATION_BACKGROUND_WIDTH,NOTIFICATION_BACKGROUND_HEIGHT,12,12);
         background.graphics.endFill();
         return background;
      }

      public static function toggleQuality(hq:Boolean) : void
      {
         if (WebMain.STAGE != null) {
            WebMain.STAGE.quality = hq ? StageQuality.HIGH : StageQuality.LOW;
         }
      }

      // Anchor stuff
      public static const ANCHOR_TOP_LEFT:int = 1;
      public static const ANCHOR_TOP_CENTER:int = 2;
      public static const ANCHOR_TOP_RIGHT:int = 3;

      public static const ANCHOR_CENTER_LEFT:int = 4;
      public static const ANCHOR_CENTER:int = 5;
      public static const ANCHOR_CENTER_RIGHT:int = 6;

      public static const ANCHOR_BOTTOM_LEFT:int = 7;
      public static const ANCHOR_BOTTOM_CENTER:int = 8;
      public static const ANCHOR_BOTTOM_RIGHT:int = 9;

      public static function positionWithAnchors(
              child:DisplayObject, parent:DisplayObject,
              childAnchorType:int, parentAnchorType:int,
              x:int = 0, y:int = 0
      ) : void {
         var childBounds:Rectangle = child.getBounds(child.parent);
         var parentBounds:Rectangle = parent.getBounds(parent.parent);

         var childAnchorPoint:Point = getAnchorPoint(childBounds, childAnchorType);
         var parentAnchorPoint:Point = getAnchorPoint(parentBounds, parentAnchorType);

         child.x = parent.x + parentAnchorPoint.x - childAnchorPoint.x + x;
         child.y = parent.y + parentAnchorPoint.y - childAnchorPoint.y + y;
      }

      private static function getAnchorPoint(bounds:Rectangle, anchorType:int) : Point {
         var anchorPoint:Point = new Point(0, 0);

         switch (anchorType) {
            case ANCHOR_TOP_CENTER:
                 anchorPoint.x = bounds.width / 2;
                 break;
            case ANCHOR_TOP_RIGHT:
               anchorPoint.x = bounds.width;
               break;
            case ANCHOR_CENTER_LEFT:
               anchorPoint.y = bounds.height / 2;
               break;
            case ANCHOR_CENTER:
               anchorPoint.x = bounds.width / 2;
               anchorPoint.y = bounds.height / 2;
               break;
            case ANCHOR_CENTER_RIGHT:
               anchorPoint.x = bounds.width;
               anchorPoint.y = bounds.height / 2;
               break;
            case ANCHOR_BOTTOM_LEFT:
               anchorPoint.y = bounds.height;
               break;
            case ANCHOR_BOTTOM_CENTER:
               anchorPoint.x = bounds.width / 2;
               anchorPoint.y = bounds.height;
               break;
            case ANCHOR_BOTTOM_RIGHT:
               anchorPoint.x = bounds.width;
               anchorPoint.y = bounds.height;
               break;
            case ANCHOR_TOP_LEFT:
            default:
                 break;
         }

         return anchorPoint;
      }
   }
}
