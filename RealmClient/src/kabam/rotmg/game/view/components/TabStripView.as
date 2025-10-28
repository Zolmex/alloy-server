package kabam.rotmg.game.view.components {
import com.company.assembleegameclient.ui.panels.itemgrids.InventoryGrid;
import com.company.util.GraphicsUtil;

import flash.display.Bitmap;
import flash.display.Graphics;
import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.IGraphicsData;
import flash.display.Sprite;
import flash.events.MouseEvent;

import io.decagames.rotmg.ui.texture.TextureParser;

import org.osflash.signals.Signal;

public class TabStripView extends Sprite {
    public static const HEIGHT:int = 203;
    public static const TAB_WIDTH:Number = 41;
    public static const TAB_HEIGHT:Number = 18;
    public static const TAB_TOP_OFFSET:int = 18;
    public static const BACKGROUND_COLOR:uint = 0x6b7874;
    public static const TAB_COLOR:uint = 0x181821;

    public var tabs:Vector.<TabView>;
    private var contents:Vector.<Sprite>;
    private var tabSprite:Sprite;
    private var containerSprite:Sprite;
    public var currentTabIndex:int;
    public const tabSelected:Signal = new Signal(String);
    public var inventoryGrid:InventoryGrid;

    public function TabStripView() {
        super();
        this.tabs = new Vector.<TabView>();
        this.contents = new Vector.<Sprite>();
        this.tabSprite = new Sprite();
        this.tabSprite.addEventListener(MouseEvent.CLICK, this.onTabSelected);
        addChild(this.tabSprite);
        this.containerSprite = new Sprite();
        this.containerSprite.y = TAB_TOP_OFFSET;
        addChild(this.containerSprite);
        var background:Bitmap = TextureParser.instance.getTexture("UI", "tab_container_background");
        this.containerSprite.addChild(background);
    }

    private function onTabSelected(e:MouseEvent):void {
        var current:TabView = null;
        var tab:TabView = e.target as TabView;
        if (tab) {
            current = this.tabs[this.currentTabIndex];
            if (current.index != tab.index) {
                current.setSelected(false);
                tab.setSelected(true);
                this.showContent(tab.index);
                this.tabSelected.dispatch(this.contents[tab.index].name);
            }
        }
    }

    public function setSelectedTab(index:uint):void {
        this.selectTab(this.tabs[index]);
    }

    private function selectTab(view:TabView):void {
        var tabFromIndex:TabView;
        if (view) {
            tabFromIndex = this.tabs[this.currentTabIndex];
            if (tabFromIndex.index != view.index) {
                tabFromIndex.setSelected(false);
                view.setSelected(true);
                this.showContent(view.index);
                this.tabSelected.dispatch(this.contents[view.index].name);
            }
        }
    }

    public function addTab(icon:Bitmap, content:Sprite) : void
    {
        var tabBG:Bitmap = TextureParser.instance.getTexture("UI", "tab_background");
        var index:int = this.tabs.length;
        var tabView:TabView = new TabView(index,tabBG,icon);
        tabView.x = 6 + index * (TAB_WIDTH + 2);
        tabView.y = 0;
        this.tabs.push(tabView);
        this.contents.push(content);
        this.tabSprite.addChild(tabView);
        this.containerSprite.addChild(content);
        if(index > 0)
        {
            content.visible = false;
        }
        else
        {
            tabView.setSelected(true);
            this.showContent(0);
            this.tabSelected.dispatch(content.name);
        }
    }

    private function showContent(index:int):void {
        var previousContent:Sprite = null;
        var currentContent:Sprite = null;
        if (index != this.currentTabIndex) {
            previousContent = this.contents[this.currentTabIndex];
            previousContent.visible = false;
            currentContent = this.contents[index];
            currentContent.visible = true;
            this.currentTabIndex = index;
        }
    }
}
}
