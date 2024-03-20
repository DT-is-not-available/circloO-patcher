using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
#line 1 "main.csx"
#line 1 "config.csx"
const string VERSION = "v1.6.3";
const string CIRCLOO_VERSION = "1.11";
const int ISBETA = 3;
const bool DEBUGMODE = false;

const string CONFIRMTEXT = "Are you sure you would like to apply these patches?";
const string WARNTEXT = "You are about to apply patches to your game file. Please create a backup of your circloO file before you do this, as these changes currently cannot be undone without a backup! Are you sure you want to continue?";
const string BETAWARNTEXT = "BETA ONLY\n\nThis version of cirQOL is only to be used on the circloO steam beta. Please use the regular version instead if you do not have the beta installed.\n\n";

// never enable this
const bool ENABLEOLDSETTER = false;
#line 2 "main.csx"
#line 1 "paths.csx"


string circloO_path;
string circloO_filepath;
if (Data == null) {
    circloO_path = "C:/Program Files (x86)/Steam/steamapps/common/circloO/";
    if (!Directory.Exists(circloO_path)) {
        circloO_path = "C:/Program Files/Steam/steamapps/common/circloO/";
    }
    circloO_filepath = circloO_path + "data.win";
    if (File.Exists(circloO_filepath)) {
        // await App.Current.MainWindow.LoadFile(circloO_path, false, false);
        // fuck you private methods, ill do what i want
        await LoadFile(circloO_filepath);
    } else {
        ScriptError("No data.win file is currently opened, and I couldn't find a valid installation of circloO on your computer. Please open a data.win and try running this script again.", "Error");
        return;
    }
} else {
    circloO_filepath = FilePath.Replace("\\", "/");
    circloO_path = Regex.Replace(circloO_filepath, @"(.+/).+\.win", "$1");
}

string backupsPath = circloO_path + "backups/";
if (!Directory.Exists(backupsPath)) {
    Directory.CreateDirectory(backupsPath);
}
#line 3 "main.csx"
#line 1 "utfileio.csx"




Task LoadFile(string path) {
    object[] args = new object[] {path, false, false};
    Task result = (Task) typeof(MainWindow).GetMethod("LoadFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
    return result;
}

Task SaveFile(string path) {
    object[] args = new object[] {path, false};
    Task result = (Task) typeof(MainWindow).GetMethod("SaveFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
    return result;
}

public partial class cirQOLdialog {
    public Task LoadFile(string path) {
        object[] args = new object[] {path, false, false};
        Task result = (Task) typeof(MainWindow).GetMethod("LoadFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
        return result;
    }

    public Task SaveFile(string path) {
        object[] args = new object[] {path, false};
        Task result = (Task) typeof(MainWindow).GetMethod("SaveFile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(App.Current.MainWindow as MainWindow, args);
        return result;
    }
}
#line 4 "main.csx"
#line 1 "patches.csx"



List<string> selectedPatches = new List<string> {};
List<string> patches = new List<string> {};
List<string> patchids = new List<string> {};
List<bool> requiredpatch = new List<bool> {};
List<Action> patchfunctions = new List<Action> {};
List<string> presets = new List<string> {};
List<List<string>> presetpatches = new List<List<string>> {};

void newPatch(string id, string name, Action patch, bool required = false) {
    patches.Add(name);
    patchids.Add(id);
    patchfunctions.Add(patch);
    requiredpatch.Add(required);
}
void newPatch(string id, Action patch, bool required = false) {
    patches.Add(id);
    patchids.Add(id);
    patchfunctions.Add(patch);
    requiredpatch.Add(required);
}
bool hasPatch(string id) {
    return selectedPatches.Contains(id) || requiredpatch[patchids.IndexOf(id)];
}

// built-in patches
// TODO: separate this into its own file

List<string> GeneralHotkeyHelp = new List<string> {};
List<string> EditorHotkeyHelp = new List<string> {"F5 - Test level"};
List<string> LevelHotkeyHelp = new List<string> {"F3 - Copy level ID (user level only)", "F6 - Toggle fixed view"};

newPatch("b-req", "cirQOL", ()=>{

/// PATCHDESC: Shows the startup message
code("gml_Object_obj_notification_Create_0").AppendGML(@"
notification_set(""cirQOL "+VERSION+" loaded with "+selectedPatches.Count+(selectedPatches.Count > 1 ? " patches!" : " patch!")+(hasPatch("b-f1") ? "\nPress F1 to show the help menu" : "")+@""", 10)
", Data);

/// PATCHDESC: Initializes (and automatically defines) any global variables. Some of these only exist because I cant create new locals (hopefully lua can fix this)
code("gml_GlobalScript_0").AppendGML(@"
global.hacked_editor = true
global.show_hitboxes = false
global.make_collectables_draw_smaller = false
global.global_view_scale = 1
global.temp = 0
global.target_instance = 0
global.animationoverride = false
", Data);

/// PATCHDESC: Swaps the draw event on obj_notification from DrawGUI to DrawGUIEnd so it displays over the main menu screen
UndertaleCode obj_notification_Draw_64 = Data.GameObjects.ByName("obj_notification").EventHandlerFor(EventType.Draw, EventSubtypeDraw.DrawGUI, Data);
UndertaleCode obj_notification_Draw_75 = Data.GameObjects.ByName("obj_notification").EventHandlerFor(EventType.Draw, EventSubtypeDraw.DrawGUIEnd, Data);
obj_notification_Draw_75.Replace(Assembler.Assemble(Disassemble(obj_notification_Draw_64), Data));
obj_notification_Draw_64.ReplaceGML("", Data);

}, true);

newPatch("e-unclamp-setter", "Editor: Remove bounds checking for the Set... window", ()=>{
/// PATCHDESC: Removes bounds checking (for the new setter)
ReplaceTextInASM("gml_GlobalScript_legui_add_variable_update_to_window", @"push.v arg.argument0
push.v arg.argument0
pushi.e -9
push.v [stacktop]self.maxValue
push.v arg.argument0
pushi.e -9
push.v [stacktop]self.minValue
pushloc.v local.val
call.i clamp(argc=3)
dup.v 1 8 ;;; this is a weird GMS2.3+ swap instruction
dup.v 0
push.v stacktop.setVar
callv.v 1
popz.v", 

@"push.v arg.argument0
pushloc.v local.val
dup.v 1 8 ;;; this is a weird GMS2.3+ swap instruction
dup.v 0
push.v stacktop.setVar
callv.v 1
popz.v", true);
});

newPatch("e-inf-nan-and-e", "Editor: Allow values to hold infinity, NaN, and E notation", ()=>{
    
    /// PATCHDESC: Replace the real value checker to be able to use e notation, infinity, negative infinity, and NaN, allows the editor to handle such values and the setter to receive them
    code("gml_GlobalScript_is_valid_real").ReplaceGML(@"
    function is_valid_real() //gml_Script_is_valid_real
    {
        if is_real(argument0)
            return 1;
        if ((string_lower(argument0) == ""inf"") || (string_lower(argument0) == ""-inf"") || (string_lower(argument0) == ""nan""))
            return 1;
        var n = string_length(argument0)
        if ((n == 0))
            return 0;
        var c = string_char_at(argument0, 1)
        if ((c == ""+"") || (c == ""-""))
            var i = 2
        else
            i = 1
        var dot = 0
        var e = 0
        var firsttime = 1
        while ((i <= n))
        {
            c = string_char_at(argument0, i)
            if ((c == ""e""))
            {
                if (e || firsttime)
                    return 0;
                e = 1
            }
            else if ((c == "".""))
            {
                if dot
                    return 0;
                dot = 1
            }
            else
            {
                firsttime = 0
                if ((ord(c) < 48) || (ord(c) > 57))
                    return 0;
            }
            i += 1
        }
        return 1;
    }
    ", Data);
});

newPatch("e-level-shape", "Editor: Change the shape of the level with F2", ()=>{
    /// PATCHDESC: level shape hotkeys in editor
    EditorHotkeyHelp.Add("F2 - Cycle through the available level shapes (circle, square, heart) (only in editor)");
    code("gml_Object_obj_leveleditor_Step_0").AppendGML(@"
    if keyboard_check_pressed(vk_f2) {
        levelShape = levelShape + 1
        switch (levelShape) {
            case 1:
                notification_set(""Set level shape to square"", 3, -1)
            break;
            case 2:
                notification_set(""Set level shape to heart"", 3, -1)
            break;
            case 3:
                levelShape = 0
                notification_set(""Set level shape to circle"", 3, -1)
            break;
        }
    }
    ", Data);

    /// PATCHDESC: Makes the heart shape export correctly from the level editor
    ReplaceTextInASM("gml_Object_obj_le_exporter_Create_0", ":[end]", @"
    pushref.i 111
    pushi.e -9
    push.v [stacktop]self.levelShape
    pushi.e 2
    cmp.i.v EQ
    bf [9999]

    :[9998]
    push.s ""shapeHeart""
    conv.s.v
    call.i gml_Script_le_export_add(argc=1)
    popz.v

    :[9999]
    
    :[end]", true);
});

newPatch("e-control-gravity", "Editor: Toggle the control gravity gamemode with F3", ()=>{
    EditorHotkeyHelp.Add("F3 - Toggle control gravity gamemode (only in editor)");
    /// PATCHDESC: controlGravity hotkey in editor
    code("gml_Object_obj_leveleditor_Step_0").AppendGML(@"
    if keyboard_check_pressed(vk_f3)
    {
        controlGravity = !controlGravity
        if controlGravity {
            notification_set(""Set control gravity: true"", 3, -1)
        } else {
            notification_set(""Set control gravity: false"", 3, -1)
        }
    }
    ", Data);

    /// PATCHDESC: Adds controlGravity property to the level editor so it can properly be saved
    code("gml_Object_obj_leveleditor_Create_0").AppendGML("controlGravity = 0", Data);

    /// PATCHDESC: Makes controlGravity export correctly from the level editor
    ReplaceTextInASM("gml_Object_obj_le_exporter_Create_0", ":[end]", @":[99999]
    pushref.i 111
    pushi.e -9
    push.v [stacktop]self.controlGravity
    pushi.e 1
    cmp.i.v EQ
    bf [9999]

    :[9998]
    push.s ""gravcontrol""
    conv.s.v
    call.i gml_Script_le_export_add(argc=1)
    popz.v

    :[9999]
    :[end]", true);
});

newPatch("e-fixed-draw", "Editor: Holding ALT will resize collectables to be a fixed size", ()=>{
    EditorHotkeyHelp.Add("ALT - Resize collectables and portals to be a fixed size");
    /// PATCHDESC: Makes view_scale referencable
    /// TODO: Figure out which object runs this script so i can just reference view_scale directly
    ReplaceTextInGML("gml_GlobalScript_le_update_view", "}", @"
        global.global_view_scale = view_scale
    }", true);

    /// PATCHDESC: Alt hotkey for showing collectables and portals at a fixed size
    code("gml_Object_obj_leveleditor_Step_0").AppendGML(@"
    if keyboard_check(vk_alt) {
        global.make_collectables_draw_smaller = true
    } else {
        global.make_collectables_draw_smaller = false
    }
    ", Data);

    /// PATCHDESC: Makes portals draw at a fixed size if fixed drawing is enabled
    ReplaceTextInGML("gml_Object_obj_le_portal_Draw_0", "draw_sprite_stretched_ext(spr_spiral, 0, (x - 20), (y - 20), 40, 40, global.maincol, 1)", @"
    var drawsize = 40
    if global.make_collectables_draw_smaller {
        drawsize = 20 / global.global_view_scale
    }
    draw_sprite_stretched_ext(spr_spiral, 0, (x - drawsize/2), (y - drawsize/2), drawsize, drawsize, global.maincol, 1)
    ", true);

    /// PATCHDESC: Makes collectables have the fixed size in the editor when selecting
    ReplaceTextInGML("gml_GlobalScript_le_tool_collectcircle_get_radius", "switch argument0", @"
    if global.make_collectables_draw_smaller {
        return 10 / global.global_view_scale
    }
    switch argument0", true);

    /// PATCHDESC: Makes collectables have the fixed size in the editor when drawing
    ReplaceTextInGML("gml_Object_obj_le_collectcircle_Draw_0", "draw_set_color(global.maincol)", @"
    draw_set_color(global.maincol)
    if global.make_collectables_draw_smaller {
        var invert = false
        if array_contains(obj_leveleditor.selectedElements, id) {
            invert = true
            draw_set_color(c_white)
        }
        if is_trigger_instead {
            var size = 8 / global.global_view_scale;
            draw_rectangle(x - size, y - size, x + size, y + size, false)
        } else
            draw_circle_fix(x, y, 10 / global.global_view_scale, 0)
        draw_set_halign(fa_center)
        draw_set_valign(fa_middle)
        draw_set_font(fnt_small)
        if invert {
            draw_set_color(global.maincol)
        } else {
            draw_set_color(c_white)
        }
        draw_text_transformed((x - 0.5), (y - 0.5), type, 1 / global.global_view_scale, 1 / global.global_view_scale, 0)
        draw_set_halign(fa_left)
        draw_set_valign(fa_top)
        draw_set_color(global.maincol)
        return 0;
    }
    ", true);

    /// PATCHDESC: Makes collectables have the fixed size select outline in the editor when drawing
    const string drawselectcircle = "draw_circle_fix(x, y, (((type == 2) ? 32 : (iif(does_not_grow, 0.69999999999999996, 1) * 20)) + 5), 0)";
    ReplaceTextInGML("gml_Object_obj_le_collectcircle_Draw_0", drawselectcircle, @"
    if global.make_collectables_draw_smaller {
        draw_circle_fix(x, y, 10 / global.global_view_scale + 3, 0)
    } else {
        "+drawselectcircle+@"
    }
    ", true);
});

newPatch("q-show-featured", "Show if your level was featured on the 'yours' tab", ()=>{
    /// PATFCDESC: Replaces the default level status text on the 'Yours' page with "Featured" if your level got featured
ReplaceTextInASM("gml_GlobalScript_menu_customlevels_init", @":[*<A*]
pushloc.v local.thisLevel
pushi.e -9
push.v [stacktop]self.moderationStatus
pushi.e 1
cmp.i.v EQ
bf [*<C*]

:[*<B*]
push.s ""Live""@*<live*
conv.s.v
b [*<D*]", @":[*A*]
pushloc.v local.thisLevel
pushi.e -9
push.v [stacktop]self.featured
pushi.e 1
cmp.i.v EQ
bf [A]

:[*B*]
push.s  ""Featured""
conv.s.v
b [*D*]

:[A]
pushloc.v local.thisLevel
pushi.e -9
push.v [stacktop]self.moderationStatus
pushi.e 1
cmp.i.v EQ
bf [*C*]

:[B]
push.s ""Live""@*live*
conv.s.v
b [*D*]", true);
});

newPatch("q-cr", "Show clear rates above each level", ()=>{
    /// PATCHDESC: Displays a clear rate above each level if not on the "yours" tab
    /// TODO: make it display on the yours tab, currently im just replacing the unused status text which gets used on yours, so i need to come up with a better solution
    ReplaceTextInASM("gml_GlobalScript_menu_customlevels_init", @":[*<A*]
    push.i *<something*
    setowner.e
    push.s """"@***
    conv.s.v
    pushi.e -1
    pushloc.v local.i
    conv.v.i
    pop.v.v [array]self.menustatustext

    :[*<B*]", @":[*A*]
    push.i *something*
    setowner.e
    pushloc.v local.thisLevel
    pushi.e -9
    push.v [stacktop]self.plays
    pushloc.v local.thisLevel
    pushi.e -9
    push.v [stacktop]self.starts
    div.v.v
    push.i 100000
    mul.i.v
    call.i ceil(argc=1)
    pushi.e 1000
    conv.i.d
    div.d.v
    call.i string(argc=1)
    push.s  ""% CR""
    conv.s.v
    add.v.v
    pushi.e -1
    pushloc.v local.i
    conv.v.i
    pop.v.v [array]self.menustatustext
    
    :[*B*]", true);
});

newPatch("q-copy-content", "Copy the current level data with F11", ()=>{
    GeneralHotkeyHelp.Add("F11 - Copy current level data");
    code("gml_Object_obj_renderer_Step_0").AppendGML(@"
        if keyboard_check_pressed(vk_f11) {
            clipboard_set_text(global.levelloadcontent)
            notification_set(""Copied global.levelloadcontent to clipboard"", 3, -1)
        }
    ", Data);
});

newPatch("q-load-content", "Load clipboard as level data content with F9", ()=>{
    GeneralHotkeyHelp.Add("F9 - Load level from clipboard");
    code("gml_Object_obj_renderer_Step_0").AppendGML(@"
        if keyboard_check_pressed(vk_f9) {
            with (obj_leveleditor) {
                instance_destroy(id)
            }
            if ((global.ispaused || global.islevelend) && (global.leveltoload != -1))
                obj_gameflowhandler.prevlevel = global.leveltoload
            global.ispaused = 0
            global.islevelend = 0
            physics_pause_enable(0)
            level_start_content(clipboard_get_text(), true)
            notification_set(""Loaded level data from clipboard"", 3, -1)
        }
    ", Data);
});

newPatch("q-edit-content", "Open the current level in the editor with F12", ()=>{
    GeneralHotkeyHelp.Add("F12 - Open the current level in the level editor");
    code("gml_Object_obj_renderer_Step_0").AppendGML(@"
        if keyboard_check_pressed(vk_f12) {
            global.levelloadcontentID = -1
            level_clear()
            instance_create(0, 0, obj_leveleditor)
            notification_set(""Entered Editor"", 3, -1)
        }
    ", Data);

    /// PATCHDESC: Makes the built-in levels openable in the level editor (the ones that were built with it anyways, only includes like half the special level pack though) and copyable via the keyboard shortcut
    ReplaceTextInGML("gml_Object_obj_createlevel_Alarm_0", "level_read(levelcode)", @"level_read(levelcode)
    global.levelloadcontent = levelcode");
});

newPatch("q-show-hitboxes", "Toggle showing hitboxes with F10", ()=>{
    GeneralHotkeyHelp.Add("F10 - Toggle show hitboxes");
    /// PATCHDESC: Changes the drawing hitboxes condition to whether hiboxes are enabled, instead of if the key combo is being held down
    ReplaceTextInGML("gml_Object_obj_renderer_Draw_73", "input_check(vk_f6) && input_check(vk_shift)", "global.show_hitboxes", true);

    /// PATCHDESC: Disables the default F10 behavior
    ReplaceTextInASM("gml_Object_obj_you_KeyPress_121", ":[0]", "exit\n:[0]", true);

    code("gml_Object_obj_renderer_Step_0").AppendGML(@"
        if keyboard_check_pressed(vk_f10) {
            global.show_hitboxes = !global.show_hitboxes
            if global.show_hitboxes {
                notification_set(""Displaying hitboxes"", 3, -1)
            } else {
                notification_set(""Hiding hitboxes"", 3, -1)
            }
        }
    ", Data);
});

newPatch("q-animation-override-hotkey", "Toggle menu animations off or on with F8", ()=>{
    GeneralHotkeyHelp.Add("F8 - Toggle menu animations");
    code("gml_Object_obj_renderer_Step_0").AppendGML(@"
        if keyboard_check_pressed(vk_f8) {
            global.animationoverride = 1 - global.animationoverride
            if global.animationoverride
                notification_set(""Menu animations: Off"", 5)
            else
                notification_set(""Menu animations: On"", 5)
        }
        if global.animationoverride {
            fadeout = 0
            with (obj_menu) {
                if gotoanim != 0
                    gotoanim = infinity
                if maingoto != -1
                    mainchangeprogress = 1
                menufadin = 1
            }
        }
    ", Data);

    /// PATCHDESC: Makes it so you cant hold down a button to keep selecting or moving selection, did this to ensure animation override didnt cause problems
    ReplaceTextInASM("gml_Object_obj_menu_Step_0", "input_check(", "input_check_pressed(", true);
});

newPatch("q-show-copy-tooltip", "Show a notification when pressing F3 to copy the level ID", ()=>{
    /// PATCHDESC: Adds shortcuts for the rest of the mod
    code("gml_Object_obj_renderer_Step_0").AppendGML(@"
    if keyboard_check_pressed(vk_f3) {
        with (obj_gameflowhandler) {
            if fromCustomLevelsMenu && instance_exists(obj_you)
            {
                notification_set(""Copied level ID"", 3, -1)
            }
        }
    }
    ", Data);
});

newPatch("e-idk", "Editor: I forgot what this patch does but it has to do with the player", ()=>{
    /// PATCHDESC: I dont even know what this one does
    ReplaceTextInASM("gml_Object_obj_le_edittool_Step_0", @"push.v self.type
    pushi.e 2
    cmp.i.v NEQ", @"push.v self.type
    pushi.e 9999
    cmp.i.v NEQ", true);
});

if (ENABLEOLDSETTER) newPatch("+e-oldsetter", "use old setter (DO NOT DO THIS I REPEAT DO NOT DO THIS IT WONT WORK)", ()=>{

/// PATCHDESC: Gets the most recently selected element and puts it in target_instance (for the old setter)
ReplaceTextInASM("gml_Object_obj_le_par_levelelement_Other_11", @"call.i array_push(argc=2)
popz.v", @"call.i array_push(argc=2)
popz.v
push.v self.id
pop.v.v global.target_instance", true);

/// PATCHDESC: Adds the Set... button to every button row created with this script (for the old setter)
ReplaceTextInGML("gml_GlobalScript_legui_add_variable_update_to_window", "var changeVarButtonSize = 60", @"var changeVarButtonSize = 60
        if global.hacked_editor
        {
            
            var button = legui_create_text_button(""Set..."", undefined, undefined, gml_Script_legui_update_variable, undefined, 90, """")
            ds_map_set(button, ""clickScriptArgument"", button)
            ds_map_set(button, ""for_object"", otherID)
            ds_map_set(button, ""get"", getter)
            ds_map_set(button, ""set"", setter)
            ds_map_set(button, ""addAmount"", 0)
            ds_map_set(button, ""min"", -infinity)
            ds_map_set(button, ""max"", infinity)
            ds_map_set(button, ""repeatable"", 1)
            ds_map_set(button, ""tooltip"", ""Set a specific value by typing in the number"")
            ds_map_set(button, ""promptOverride"", true)

            legui_toolbar_add_button(windowToolbar, button)
        }
", true);

/// PATCHDESC: create a function that runs the button getter when the button is passed to it (for the old setter)
code("gml_GlobalScript_cirqol_runbuttongetter").ReplaceGML("function cirqol_runbuttongetter(argument0) {}", Data);
code("gml_GlobalScript_cirqol_runbuttongetter").Replace(Assembler.Assemble(@"
:[0]
b [5]

> gml_Script_cirqol_runbuttongetter (locals=0, argc=2)
:[1]
pushglb.v global.target_instance
pushi.e -9
pushenv [4]

:[withstatement]
push.s ""set""
conv.s.v
push.v arg.argument0
call.i ds_map_find_value(argc=2)
call.i is_method(argc=1)
conv.v.b
bf [3]

:[2]
push.v arg.argument1
call.i @@This@@(argc=0)
push.s ""set""
conv.s.v
push.v arg.argument0
call.i ds_map_find_value(argc=2)
callv.v 1
popz.v
b [4]

:[3]
push.v arg.argument1
push.s ""set""
conv.s.v
push.v arg.argument0
call.i ds_map_find_value(argc=2)
call.i script_execute(argc=2)
popz.v

:[4]
popenv [withstatement]
exit.i

:[5]
push.i gml_Script_cirqol_runbuttongetter
conv.i.v
pushi.e -1
conv.i.v
call.i method(argc=2)
dup.v 0
pushi.e -1
pop.v.v [stacktop]self.cirqol_runbuttongetter
popz.v

:[end]
", Data));

/// PATCHDESC: Create a function that when run, shows the debug get string window to set values with (for the old setter)
code("gml_GlobalScript_cirqol_showsetter").ReplaceGML(@"
function cirqol_showsetter(argument0, argument1) {
    cirqol_runbuttongetter(argument1, real(get_string(""Type in a new value to be used"", argument0)))
}
", Data);

/// PATCHDESC: Create a function that when run, closes the setter window and goes back to the previous window (for the old setter)
code("gml_GlobalScript_cirqol_cancelsetter").ReplaceGML(@"
function cirqol_cancelsetter() {
    with obj_le_gui {
        legui_destroy_window()
        window = global.prevWindow
        windowRelatedTo = global.prevWindowRelatedTo
        global.prevWindow = -1
        global.prevWindowRelatedTo = undefined
    }
}
", Data);

/// PATCHDESC: Create a function that when run, closes the setter window, sets the value to what was typed in, and goes back to the previous window (for the old setter)
code("gml_GlobalScript_cirqol_confirmsetter").ReplaceGML(@"
function cirqol_confirmsetter() {
    if is_valid_real(keyboard_string) {
        with obj_le_gui {
            legui_destroy_window()
            window = global.prevWindow
            windowRelatedTo = global.prevWindowRelatedTo
            global.prevWindow = -1
            global.prevWindowRelatedTo = undefined
        }
        cirqol_runbuttongetter(global.cirqol_tempbutton, real(keyboard_string))
    } else {
        notification_set(""The provided value is not a real number"", 10, -1)
    }
}
", Data);

/// PATCHDESC: Create a function that when run, pastes the clipboard text into the input box (for the old setter)
code("gml_GlobalScript_cirqol_paste").ReplaceGML(@"
function cirqol_paste() {
    if clipboard_has_text()
        keyboard_string = clipboard_get_text()
}
", Data);

/// PATCHDESC: Create a function that when run, clears the input box (for the old setter)
code("gml_GlobalScript_cirqol_clear").ReplaceGML(@"
function cirqol_clear() {
    keyboard_string = """"
}
", Data);

/// PATCHDESC: Disable view controls while the setter window is open (for the old setter)
ReplaceTextInASM("gml_Object_obj_le_viewhandler_Step_0", @"bf [70]", @"bf [70]
pushref.i 114
pushi.e -9
push.v [stacktop]self.windowRelatedTo
push.s ""cirqol_setter""
cmp.s.v NEQ
bf [70]", true);

/// PATCHDESC: Disable button shortcuts while the setter window is open (for the old setter)
ReplaceTextInGML("gml_GlobalScript_legui_step_button", @"(obj_le_gui.windowRelatedTo != ""save"")", @"
(obj_le_gui.windowRelatedTo != ""save"") &&
(obj_le_gui.windowRelatedTo != ""cirqol_setter"")
", true);

/// PATCHDESC: Creates a new function that shows the setter window (for the old setter)
code("gml_GlobalScript_cirqol_showsetter").ReplaceGML(@"
function cirqol_showsetter(argument0, argument1) {
    with obj_le_gui {
        global.cirqol_tempbutton = argument1
        global.prevWindow = window
        global.prevWindowRelatedTo = windowRelatedTo
        window = -1
        windowRelatedTo = undefined
        if ((windowRelatedTo == ""cirqol_setter"")) {
            legui_destroy_window()
            return;
        }
        legui_create_basic_window(""Enter a new value"")
        windowRelatedTo = ""cirqol_setter""
        legui_window_add_element(legui_create_input_field("""", 320))
        keyboard_string = string(argument0)
        legui_window_add_spacing()
        global.hackedEditorWindowToolbar = legui_create_toolbar()
        legui_toolbar_add_button(global.hackedEditorWindowToolbar, legui_create_text_button(""Paste"", undefined, undefined, gml_Script_cirqol_paste, undefined, 155, ""Pastes whatever you have copied to the clipboard""))
        legui_toolbar_add_spacing_auto(global.hackedEditorWindowToolbar, 155)
        legui_toolbar_add_button(global.hackedEditorWindowToolbar, legui_create_text_button(""Set"", undefined, undefined, gml_Script_cirqol_confirmsetter, undefined, 155, """"))
        legui_window_add_element(global.hackedEditorWindowToolbar)
        legui_window_add_spacing()
        legui_window_add_element(legui_create_text_button(""Cancel"", undefined, undefined, gml_Script_cirqol_cancelsetter, undefined, 320, """"))
        legui_reposition_window()
    }
}
", Data);

/// PATCHDESC: Makes it so that the setter button actually shows the setter window instead of just adding 0 (for the old setter)
ReplaceTextInASM("gml_GlobalScript_legui_update_variable", @"pushloc.v local.newValue
push.d -0.0001
cmp.d.v GT
bf [7]", @"
push.s ""promptOverride""
conv.s.v
pushloc.v local.button
call.i ds_map_find_value(argc=2)
conv.v.b
bf [39999]

:[29999]
pushloc.v local.button
pushloc.v local.currentValue
call.i gml_Script_cirqol_showsetter(argc=2)
popz.v

:[39999]

pushloc.v local.newValue
push.d -0.0001
cmp.d.v GT
bf [7]", true);

});

newPatch("+e-flashing-underscore", "Editor: Make text inputs have a flashing underscore", ()=>{
    /// PATCHDESC: Make sure that current_time exists
    /// TODO: Do this with all newly defined variables, dont just rely on GML to auto define them
    Data.Variables.EnsureDefined("current_time", UndertaleInstruction.InstanceType.Self, true, Data.Strings, Data);

    /// PATCHDESC: Make it so the input field has a flashing underscore after it
    ReplaceTextInASM("gml_GlobalScript_legui_create_input_field", @"pushbltn.v builtin.keyboard_string
ret.v", @"pushbltn.v builtin.current_time
    pushi.e 500
    mod.i.v
    pushi.e 250
    cmp.i.v GT
    bf [399]

    :[299]
    pushbltn.v builtin.keyboard_string
    push.s ""_""
    add.s.v
    ret.v

    :[399]
    pushbltn.v builtin.keyboard_string
    ret.v", true);
});

newPatch("e-inputbox-morevals", "Editor: Allow typing E notation into the Set... window", ()=>{
    ReplaceTextInASM("gml_GlobalScript_legui_create_input_field", @"pushloc.v local.char
push.s "".""@*<refDot*
cmp.s.v NEQ
bt [*<B*]

:[*<A*]
pushloc.v local.str
push.s "".""@*>refDot*
conv.s.v
call.i string_count(argc=2)
pushi.e 1
cmp.i.v GT
b [*<C*]

:[*>B*]
push.e 1

:[*>C*]
bf [*<D*]", 
// wioth
@"
pushloc.v local.char
push.s "".""@*refDot*
cmp.s.v NEQ
bt [*B*]

:[*A*]
pushloc.v local.str
push.s "".""@*refDot*
conv.s.v
call.i string_count(argc=2)
pushi.e 1
cmp.i.v GT
b [*C*]

:[*B*]
push.e 1

:[*C*]
bf [*D*]

pushloc.v local.char
push.s ""e""
cmp.s.v NEQ
bt [B]

:[A]
pushloc.v local.str
push.s ""e""
conv.s.v
call.i string_count(argc=2)
pushi.e 1
cmp.i.v GT
b [C]

:[B]
push.e 1

:[C]
bf [*D*]
", true);
});

newPatch("e-multiple-player", "Editor: Allow placing multiple player objects", ()=>{
    /// PATCHDESC: Allows you to place more than one player into the editor
ReplaceTextInASM("gml_Object_obj_le_edittool_Step_0", @":[247]
push.v self.type
pushi.e 2", @":[247]
push.v self.type
pushi.e 9999", true);
});

newPatch("ex-q-settings", "[Experimental] Add a cirQOL button to the main menu with related settings", ()=>{

/// PATCHDESC: Makes the main menu editable via GML again, replaces the weird @@Global@@() calls with the standard global namespace
ReplaceTextInGML("gml_GlobalScript_menu_main_init", "@@Global@@()", "global", true);

/// PATCHDESC: Adds the cirQOL button to the main menu
ReplaceTextInGML("gml_GlobalScript_menu_main_init", @"menutext[curr] = ""Settings""
    menufont[curr] = fnt_biggish_semibold
    focusonmobile[curr] = 0
    curr += 1", @"

menutext[curr] = ""Settings""
menufont[curr] = fnt_biggish_semibold
focusonmobile[curr] = 0
curr += 1

menutext[curr] = ""[EXTR=version "+VERSION+@"]cirQOL""
menufont[curr] = fnt_biggish_semibold_new
focusonmobile[curr] = 0
curr += 1

", true);
ReplaceTextInGML("gml_GlobalScript_menu_main_init", "gotoanimtype[4] = 0", "gotoanimtype[4] = 0; gotoanimtype[5] = 0", true);
ReplaceTextInGML("gml_GlobalScript_menu_main_init", "mainnumber = 4", "mainnumber = 5", true);

/// PATCHDESC: Makes the menu option actually do something (currently just goes to normal settings)
ReplaceTextInGML("gml_GlobalScript_menu_main_option_handle", @"if ((main == curr))
        menu_settings_init()
    curr += 1", @"

if ((main == curr))
    menu_settings_init()
curr += 1
if ((main == curr))
    menu_settings_init()
curr += 1

", true);

});

newPatch("b-f1", "Show a help menu when you press F1", ()=>{
/// PATCHDESC: Adds the F1 menu
code("gml_Object_obj_renderer_Step_0").AppendGML(@"
if keyboard_check_pressed(vk_f1) {
    if obj_notification.notification_time == infinity
        obj_notification.notification_time = 1
    else
    notification_set("""+
        new string('\n', 50) +
        
        "General hotkeys:\\n\\n" + String.Join("\n", GeneralHotkeyHelp) + "\n\n" +
        "While playing a level:\\n\\n" + String.Join("\n", LevelHotkeyHelp) + "\n\n" +
        "While in the level editor:\\n\\n" + String.Join("\n", EditorHotkeyHelp) + "\n\n" +

        "Press F1 again to hide this popup\n" +
    
    @""", infinity)
}
", Data);
});
#line 5 "main.csx"
#line 1 "presets.csx"



List<string> newPreset(string name, List<string> patches) {
    presets.Add(name);
    presetpatches.Add(patches);
    return patches;
}
List<string> newPreset(string name) {
    return newPreset(name, new List<string> {});
}
void applyPreset(int id) {
    if (presetpatches.Count > id) {
        List<string> selectedPatches = new List<string> (presetpatches[id]);
    }
}

// built-in presets
// TODO: make this dynamically loaded

newPreset("Everything", patchids);
List<string> experimental = newPreset("Experimental Features");
List<string> recommended = newPreset("Recommended");
List<string> QOLmods = newPreset("QOL modifications only");
List<string> editmods = newPreset("Editor modifications only");
newPreset("Nothing");
for (int i = 0; i < patchids.Count; i++) {
    string id = patchids[i];
    if (id.StartsWith("e-")) {
        editmods.Add(id);
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("q-")) {
        QOLmods.Add(id);
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("b-")) {
        QOLmods.Add(id);
        editmods.Add(id);
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("r-")) {
        recommended.Add(id);
        experimental.Add(id);
    }
    if (id.StartsWith("ex-")) {
        experimental.Add(id);
    }
}
#line 6 "main.csx"
#line 1 "dialog.csx"






public class DMenuItem {
    public string Label;
    public Action Handler;
    public DMenuItem(string label, Action handler) {
        this.Label = label;
        this.Handler = handler;
    }
}

public partial class cirQOLdialog : System.Windows.Forms.Form
{
    public System.Windows.Forms.CheckedListBox patchlist_ui;
    public System.Windows.Forms.Button AcceptButton;
    public System.Windows.Forms.Button CancelButton;
    public System.Windows.Forms.ListBox installMode_ui;
    public System.Windows.Forms.MenuStrip Menu;
    public System.ComponentModel.Container components;
    public List<string> selectedPatches;
    public List<string> patches;
    public List<string> patchids;
    public List<string> presets;
    public List<bool> requiredpatch;
    public List<List<string>> presetpatches;
    public bool isinput;
    public int offset;
    public string circloO_filepath;
    public UndertaleData Data;

    public cirQOLdialog(List<string> selectedPatches, List<string> patches, List<string> patchids, List<string> presets, List<List<string>> presetpatches, string circloO_filepath, UndertaleData Data, List<bool> requiredpatch) {
        this.Data = Data;
        this.circloO_filepath = circloO_filepath;
        this.selectedPatches = selectedPatches;
        this.patches = patches;
        this.patchids = patchids;
        this.presets = presets;
        this.presetpatches = presetpatches;
        this.offset = 0;
        this.requiredpatch = requiredpatch;

        InitializeComponent();

        for (int i = this.offset; i < this.patches.Count; i++) {
            patchlist_ui.Items.Add(this.patches[i]);
        }

        for (int i = 0; i < this.presets.Count; i++) {
            installMode_ui.Items.Add(this.presets[i]);
        }

        installMode_ui.SelectedIndex = presets.IndexOf("Recommended");

        this.isinput = true;

        // Changes the selection mode from double-click to single click.
        patchlist_ui.CheckOnClick = true;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.CenterToScreen();
    }

    public void updatePatchChecklist() {
        this.isinput = false;
        for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
            if (this.requiredpatch[i + this.offset]) {
                patchlist_ui.SetItemCheckState(i, CheckState.Indeterminate);
            } else {
                patchlist_ui.SetItemChecked(i, presetpatches[installMode_ui.SelectedIndex].Contains(this.patchids[i + this.offset]));
            }
        }
        this.isinput = true;
    }

    public void updatePatchChecklist(List<string> preset) {
        this.isinput = false;
        for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
            patchlist_ui.SetItemChecked(i, preset.Contains(this.patchids[i + this.offset]));
        }
        this.isinput = true;
        try {
            if (this.isinput) this.installMode_ui.ClearSelected();
        } catch (Exception error) {

        }
    }

    protected override void Dispose( bool disposing )
    {
        if( disposing )
        {
            if (components != null) 
            {
                components.Dispose();
            }
        }
        base.Dispose( disposing );
    }

    public void patchlist_ui_ItemCheck(object sender, ItemCheckEventArgs e) {
        if (e.CurrentValue == CheckState.Indeterminate) e.NewValue = CheckState.Indeterminate;
    }

    public void styleButton(System.Windows.Forms.Button button, bool disabled = false) {
        button.FlatStyle = FlatStyle.Flat;

        if (disabled) {
            button.BackColor = Color.LightGray;
            button.ForeColor = Color.DarkGray;
        } else {
            button.BackColor = Color.FromArgb(234, 234, 234);
            button.ForeColor = Color.Black;
        }
        
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 234, 255);

        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 214, 235);
    }

    public void styleButtonGreen(System.Windows.Forms.Button button, bool disabled = false) {
        button.FlatStyle = FlatStyle.Flat;

        if (disabled) {
            button.BackColor = Color.LightGray;
            button.ForeColor = Color.DarkGray;
        } else {
            button.BackColor = Color.FromArgb(234, 234, 234);
            button.ForeColor = Color.Black;
        }

        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 255, 200);

        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 235, 180);
    }

    public void InitializeComponent()
    {
        this.BackColor = Color.White;
        this.initMenuBar();
        this.ShowIcon = false;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.components = new System.ComponentModel.Container();
        this.patchlist_ui = new System.Windows.Forms.CheckedListBox();
        this.installMode_ui = new System.Windows.Forms.ListBox();
        this.AcceptButton = new System.Windows.Forms.Button();
        this.CancelButton = new System.Windows.Forms.Button();
        this.patchlist_ui.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.itemCheck);
        this.patchlist_ui.IntegralHeight = false;
        this.patchlist_ui.BorderStyle = BorderStyle.FixedSingle;
        this.patchlist_ui.Location = new System.Drawing.Point(220, 110);
        this.patchlist_ui.Size = new System.Drawing.Size(400, 250);
        this.patchlist_ui.TabIndex = 1;
        this.patchlist_ui.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.patchlist_ui_ItemCheck);
        this.patchlist_ui.ItemHeight = 20;
        this.installMode_ui.BorderStyle = BorderStyle.FixedSingle;
        this.installMode_ui.IntegralHeight = false;
        this.installMode_ui.Location = new System.Drawing.Point(10, 110);
        this.installMode_ui.Size = new System.Drawing.Size(200, 250);
        this.installMode_ui.TabIndex = 0;
        this.installMode_ui.SelectedValueChanged += new EventHandler(this.presetChanged);
        this.installMode_ui.ItemHeight = 20;
        this.AcceptButton.Enabled = true;
        this.AcceptButton.Location = new System.Drawing.Point(220, 370);
        this.AcceptButton.Size = new System.Drawing.Size(400, 30);
        this.AcceptButton.TabIndex = 3;
        this.AcceptButton.Text = "Patch!";
        this.AcceptButton.Click += new System.EventHandler(this.AcceptButton_Click);
        this.styleButtonGreen(this.AcceptButton);
        this.CancelButton.Enabled = true;
        this.CancelButton.Location = new System.Drawing.Point(10, 370);
        this.CancelButton.Size = new System.Drawing.Size(200, 30);
        this.CancelButton.TabIndex = 2;
        this.CancelButton.Text = "Cancel";
        this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
        this.styleButton(this.CancelButton);
        this.ClientSize = new System.Drawing.Size(630, 440);
        this.Controls.AddRange(new System.Windows.Forms.Control[] {
            this.installMode_ui,
            this.AcceptButton,
            this.CancelButton,
            this.patchlist_ui
        });
        this.Text = "cirQOL Patcher "+VERSION;
        
        makeLabel(@"cirQOL is a circloO mod that aims to give small QOL improvements in both the main game and the level editor.
You can customize the patches that get applied below, or just hit 'Patch!' to apply the defaults.", 10, 40);

        makeLabel("Presets:", 10, 90);
        makeLabel("Customize Patches:", 220, 90);

        Label credit = new Label();
        credit.Text = "Created by DT";
        credit.AutoSize = true;
        credit.Location = new Point(315 - credit.Width / 2, 415);
        this.Controls.Add(credit);
        
        this.updatePatchChecklist();
    }

    public Label makeLabel(string text, int x, int y) {
        Label label = new Label(); 
        label.Text = text; 
        label.Location = new Point(x, y); 
        label.AutoSize = true;
        this.Controls.Add(label);
        return label;
    }

    public void presetChanged(object sender, EventArgs e)
    {
        this.updatePatchChecklist();
    }

    public bool Warn(string text, string title="Warning!") {
        return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
    }

    public bool Question(string text, string title="Confirm?") {
        return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    public void Inform(string text, string title="DT forgot to change the default title of the information box you should make fun of them") {
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void Error(string text, string title="Error!") {
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Adds the string if the text box has data in it.
    async public void AcceptButton_Click(object sender, System.EventArgs e)
    {
        if (this.patchlist_ui.CheckedItems.Count == 0) return;
        UndertaleData Data = this.Data;
        string circloO_path = Regex.Replace(this.circloO_filepath, @"(.+/).+\.win", "$1");
        string backupsPath = circloO_path + "backups/";
        string backupName = backupsPath + Data.GeneralInfo.FileName.Content + "-" + Data.GeneralInfo.Timestamp + ".win";
        bool hasbackup = false;
        bool skipwarn = false;
        bool shouldcontinue = false;

        Func<bool, bool> doWarnings = (dontgiveup) => {
            if (dontgiveup) {
                if (skipwarn) {
                    if (ISBETA != 0) {
                        return this.Question(BETAWARNTEXT+CONFIRMTEXT);
                    } else return true;
                } else {
                    if (hasbackup) {
                        if (ISBETA != 0) {
                            return this.Question(BETAWARNTEXT+CONFIRMTEXT);
                        } else {
                            return this.Question(CONFIRMTEXT);
                        }
                    } else {
                        if (ISBETA != 0) {
                            return this.Warn(BETAWARNTEXT+WARNTEXT);
                        } else {
                            return this.Warn(WARNTEXT);
                        }
                    }
                }
            } else return false;
        };

        if (Data.Code.ByName("hasbeenmodded") != null) {
            if (File.Exists(backupName)) {
                if (shouldcontinue = doWarnings(this.Question("You must restore your previously created backup in order to modify the game. Do this and apply changes?", "Restore backup?"))) {
                    hasbackup = true;
                    skipwarn = true;
                    File.Copy(backupName, circloO_filepath, true);
                    // await App.Current.MainWindow.LoadFile(circloO_path, false, false);
                    // fuck you private methods, ill do what i want
                    await LoadFile(circloO_filepath);
                } else {
                    return;
                }
            } else {
                MessageBox.Show("I couldn't find an automatically created backup in your backups folder. If you have a manually created backup, please copy it to the game files and try again.", "Backup not found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        } else {
            if (!File.Exists(backupName)) {
                if (shouldcontinue = doWarnings(this.Question("Would you like to automatically create a backup of your game? It is recommended that you hit yes so the tool can automatically revert the changes if need be.", "Create backup?"))) {
                    File.Copy(circloO_filepath, backupName, true);
                    hasbackup = true;
                }
            }
        }

        if (!shouldcontinue) return;
        
        for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
            if (this.patchlist_ui.CheckedItems.Contains(this.patchlist_ui.Items[i])) {
                this.selectedPatches.Add(this.patchids[i+this.offset]);
            }
        }
        this.Dispose(true);
        this.Close();
    }

    // Adds the string if the text box has data in it.
    public void CancelButton_Click(object sender, System.EventArgs e)
    {
        this.Dispose(true);
        this.Close();
    }

    public void initMenuBar() {

        MenuStrip ms = new MenuStrip();
        ms.BackColor = Color.FromArgb(244, 244, 244);
        this.Menu = ms;

        this.addMenu("File", new DMenuItem[] {
            new DMenuItem("Import patch configuration", () => {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Text file|*.txt";
                openDialog.Title = "Export patch configuration";
                openDialog.ShowDialog();
                if(openDialog.FileName != "") {
                    List<string> importpatchids = new List<string> (File.ReadAllText(openDialog.FileName).Split("\n"));
                    this.updatePatchChecklist(importpatchids);
                    this.Inform("Patch configuration imported!", "You can close this window now");
                }
            }),
            new DMenuItem("Export patch configuration", () => {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Text file|*.txt";
                saveDialog.Title = "Export patch configuration";
                saveDialog.ShowDialog();
                if(saveDialog.FileName != "") {
                    List<string> exportpatchids = new List<string> {};
                    for (int i = 0; i < this.patchlist_ui.Items.Count; i++) {
                        if (this.patchlist_ui.CheckedItems.Contains(this.patchlist_ui.Items[i])) {
                            exportpatchids.Add(this.patchids[i+this.offset]);
                        }
                    }
                    File.WriteAllText(saveDialog.FileName, String.Join("\n", exportpatchids));
                    this.Inform("Current patch configuration exported!", "You can close this window now");
                }
            }),
        });

        this.addMenu("Help", new DMenuItem[] {
            new DMenuItem("About", () => {
                string computedversion = (ISBETA != 0 ? CIRCLOO_VERSION + " beta "+ISBETA : CIRCLOO_VERSION);
                this.Inform(
$@"cirQOL Patcher {VERSION}
Made for circloO version {computedversion}
Fun fact I started making this right after surgery because I had nothing better to do",

"About");
            }),
            new DMenuItem("Credits", () => {
                this.Inform(
$@"cirQOL and cirQOL Patcher created by DT with lots of love
and pain
lots of pain
(c# is the worst language i have ever had to deal with)

Thanks Ewoly for the name idea because quite frankly naming this thing 'circloO editor hack and mod patcher' is not a good idea",

"Credits");
            }),
        });

        ms.Dock = DockStyle.Top;
        ms.Padding = new Padding(0);

        this.Controls.Add(ms);
    }

    public void addMenu(string menuname, DMenuItem[] items) {
        ToolStripMenuItem menu = new ToolStripMenuItem(menuname);
        this.Menu.MdiWindowListItem = menu;
        this.Menu.Items.Add(menu);
        for (int i = 0; i < items.Length; i++) {
            Action Handler = items[i].Handler;
            menu.DropDownItems.Add(new ToolStripMenuItem(items[i].Label, null, (object sender, System.EventArgs e) => {
                // MessageBox.Show("thingy was clicked!!!", "hurrah!!!!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Handler();
            }));
        }
        ((ToolStripDropDownMenu)(menu.DropDown)).ShowImageMargin = false;
        ((ToolStripDropDownMenu)(menu.DropDown)).ShowCheckMargin = true;
    }

    // when checked
    public void itemCheck(object sender, ItemCheckEventArgs e) {
        int change = e.NewValue == CheckState.Checked ? 1 : -1;
        try {
            if (this.isinput) this.installMode_ui.ClearSelected();
        } catch (Exception error) {

        }
        bool enableAcceptButton = this.patchlist_ui.CheckedItems.Count + change > 0;
        this.AcceptButton.Enabled = true;
        this.styleButtonGreen(this.AcceptButton, false);
    }
}
#line 7 "main.csx"






/*
    BIG LIST OF TODOS:
    - Make the ReplaceTextInASM function allow for wildcard numbers (something like ** when finding any number, and *1* to find a matching thing that could be any number, ie for assembly labels)
    - Condense patches into their own functions, then insert those functions into the main code which will allow for the next item
    - Easier removal and updating of cirQOL than "yeah just nuke your data.win file trust me it'll be fine"
    - Make this script auto generated because this is becoming a nightmare to edit and im tired of using "" to represent " in multiline strings and quite frankly theres a better way of doing this
*/
/* ideas:
    -   zip file with custom patches for adding custom patches
    -   patches involve 'hooks': hooks can be created and inserted anywhere in the code, and patches can latch onto that without worrying about conflicting with each other
    -   find some better way to replace and deactivate certain parts of code
*/

public Form form = new cirQOLdialog(selectedPatches, patches, patchids, presets, presetpatches, circloO_filepath, Data, requiredpatch);
form.ShowDialog();
if (selectedPatches.Count == 0) return;

string PATCHLIST = String.Join("\n", selectedPatches);

if (DEBUGMODE) {
    ScriptMessage("BEEP BOOP I DID IT\n\n"+PATCHLIST);
}

code("hasbeenmodded");

for (int i = 0; i < patchids.Count; i++) {
    if (requiredpatch[i] || selectedPatches.Contains(patchids[i])) {
        patchfunctions[i]();
    }
}

/// PATCHDESC: just read the fucking line of code
Data.GeneralInfo.DisplayName = Data.Strings.MakeString("cirQOL");

// await App.Current.MainWindow.SaveFile(circloO_path);
// fuck you private methods, ill do what i want
await SaveFile(circloO_filepath);

ScriptMessage("Done! Game file has been modified, all you need to do now is run circloO.");

/// FUNCDEFS:

string GetReplacerRegex(string keyword) {
    Regex reg = new Regex(@"\\\*(\w+)\\\*");
    return Regex.Replace(
        Regex.Replace(
            Regex.Replace(
                Regex.Escape(
                    Regex.Replace(
                        keyword, @"^[^\S\n]+|[^\S\n]+$", "", RegexOptions.Multiline
                    )
                ),  @"\\\*\\\*\\\*", @"\-?[\d\.]+", RegexOptions.None
            ), @"\\\*<(\w+)\\\*", @"(?'$1'\-?[\d\.]+)", RegexOptions.None
        ), @"\\\*>(\w+)\\\*", @"\k'$1'", RegexOptions.None
    );
}

string GetReplacementRegex(string replacement) {
    return Regex.Replace(
        Regex.Replace(
            replacement.Replace("$", "$$"), @"^[^\S\n]+|[^\S\n]+$", "", RegexOptions.Multiline
        ), @"\*(\w+)\*", "$${$1}", RegexOptions.Multiline
    );
}

string GetPassBack(string decompiled_text, string keyword, string replacement, bool case_sensitive = false, bool isRegex = false)
{
    decompiled_text = decompiled_text.Replace("\r\n", "\n");
    keyword = keyword.Replace("\r\n", "\n");
    replacement = replacement.Replace("\r\n", "\n");
    string passBack;
    if (!isRegex)
    {
        if (DEBUGMODE) ScriptMessage(GetReplacerRegex(keyword));
        if (DEBUGMODE) Clipboard.SetText(GetReplacerRegex(keyword));
        if (DEBUGMODE) ScriptMessage(GetReplacementRegex(replacement));
        if (case_sensitive)
            passBack = Regex.Replace(decompiled_text, GetReplacerRegex(keyword), GetReplacementRegex(replacement), RegexOptions.None);
        else
            passBack = Regex.Replace(decompiled_text, GetReplacerRegex(keyword), GetReplacementRegex(replacement), RegexOptions.IgnoreCase);
    }
    else
    {
        if (case_sensitive)
            passBack = Regex.Replace(decompiled_text, keyword, replacement, RegexOptions.None);
        else
            passBack = Regex.Replace(decompiled_text, keyword, replacement, RegexOptions.IgnoreCase);
    }
    return passBack;
}
void ReplaceTextInASM(string codeName, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null)
{
    UndertaleCode code = Data.Code.ByName(codeName);
    if (code is null)
        throw new ScriptException($"No code named \"{codeName}\" was found!");

    ReplaceTextInASM(code, keyword, replacement, caseSensitive, isRegex, context);
}
void ReplaceTextInASM(UndertaleCode code, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null)
{
    if (code.ParentEntry is not null)
        return;

    EnsureDataLoaded();

    string passBack = "";
    string codeName = code.Name.Content;
    GlobalDecompileContext DECOMPILE_CONTEXT = context is null ? new(Data, false) : context;

    try
    {
        // It would just be recompiling an empty string and messing with null entries seems bad
        if (code is null)
            return;
        string originalCode = code.Disassemble(Data.Variables, Data.CodeLocals.For(code));
        // ScriptMessage(originalCode.Substring(0, 1000));
        passBack = GetPassBack(originalCode, keyword, replacement, caseSensitive, isRegex);
        // No need to compile something unchanged
        if (passBack == originalCode) {
            ScriptMessage("its the same");
            return;
        }
        var instructions = Assembler.Assemble(passBack, Data);
        code.Replace(instructions);
    }
    catch (Exception exc)
    {
        if (ScriptQuestion("Error during ASM code replacement:\n" + exc.ToString() + "\n\nCopy passback?")) {
            Clipboard.SetText(passBack);
        };
        throw new Exception("a");
    }
}
string Disassemble(UndertaleCode code) {
    return code.Disassemble(Data.Variables, Data.CodeLocals.For(code));
}
UndertaleCode code(string codeName, bool makeIfNotExists = true) {
    UndertaleCode c = Data.Code.ByName(codeName);
    if (c == null && makeIfNotExists) {
        c = new UndertaleCode();
        c.Name = Data.Strings.MakeString(codeName);
        Data.Code.Add(c);
    }
    return c;
}