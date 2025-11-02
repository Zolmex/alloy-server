package kabam.rotmg.ui.view
{
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.StatusBar;
import flash.display.Sprite;
import flash.events.Event;

public class StatMetersView extends Sprite
{


    private var expBar_:StatusBar;

    private var fameBar_:StatusBar;

    private var hpBar_:StatusBar;

    private var mpBar_:StatusBar;

    private var msBar_:StatusBar;

    public function StatMetersView()
    {
        super();
        this.expBar_ = new StatusBar(176,16,5931045,5526612,"Lvl X");
        this.fameBar_ = new StatusBar(176,16,14835456,5526612,"Fame");
        this.hpBar_ = new StatusBar(176,16,14693428,5526612,"HP");
        this.mpBar_ = new StatusBar(176,16,6325472,5526612,"MP");
        this.msBar_ = new StatusBar(176,16,0xbf6cff,0x3526ad,"MS");
        this.hpBar_.y = 18;
        this.mpBar_.y = 54;
        this.msBar_.y = 36;
        this.expBar_.visible = true;
        this.fameBar_.visible = false;
        addChild(this.expBar_);
        addChild(this.fameBar_);
        addChild(this.hpBar_);
        addChild(this.mpBar_);
        addChild(this.msBar_)
    }

    public function update(player:Player) : void
    {
        var lvlText:String = "Lvl " + player.level_;
        if(lvlText != this.expBar_.labelText_.text)
        {
            this.expBar_.labelText_.text = lvlText;
            this.expBar_.labelText_.updateMetrics();
        }
        if(player.level_ != player.levelCap)
        {
            if(!this.expBar_.visible)
            {
                this.expBar_.visible = true;
                this.fameBar_.visible = false;
            }
            this.expBar_.draw(player.experience_,player.nextLevelXp_);
        }
        else
        {
            if(!this.fameBar_.visible)
            {
                this.fameBar_.visible = true;
                this.expBar_.visible = false;
            }
            this.fameBar_.draw(player.charFame_,player.nextClassQuestFame_);
        }
        this.hpBar_.draw(player.hp,player.maxHP);
        this.mpBar_.draw(player.mp,player.maxMP);
        this.msBar_.draw(player.ms, player.maxMS);
    }
}
}