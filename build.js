const fs = require("fs");
const path = require("path");
const project = require("./project.json");

try {
    fs.mkdirSync(path.dirname(project.outfile), {recursive: true});
} catch (e) {
    // already exists ig
}

const srcpath = path.resolve(project.src);

function processFile(fp, using) {
    let toplevelfile = false
    if (!using) {
        using = {};
        toplevelfile = true;
    }
    const filepath = fp;
    const lines = fs.readFileSync(filepath, "utf-8").split(/\r?\n/gm);
    let loads = true;
    for (let i = 0; i < lines.length; i++) {
        const parts = lines[i].trimStart().trimEnd().split(/\s+/);
        if (parts.length == 0) continue;
        if (parts[0] == "using") {
            using[parts[1]] = true
            lines[i] = ""
        }
        if (parts[0] == "#load") {
            if (loads) {
                const newpath = JSON.parse(parts[1]);
                const p = path.resolve(path.dirname(fp), newpath);
                lines[i] = processFile(p, using)+"\n#line "+(i+2)+` "${path.relative(srcpath, filepath)}"`;
            } else {
                console.error("Cannot include a #load after the first line of code");
                process.exit(1);
            }
        } else if (parts[0][0] != "#") {
            loads = false;
        }
    }
    let extrastr = ""
    if (toplevelfile) for (const [k, v] of Object.entries(using)) {
        extrastr += `using ${k}\n`
    }
    return extrastr + `#line 1 "${path.relative(srcpath, filepath)}"\n`+lines.join("\n");
}

fs.writeFileSync(project.outfile, processFile(path.join(srcpath, project.infile)), "utf8");