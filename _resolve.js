const fs = require('fs');
const cfg = JSON.parse(fs.readFileSync('d:/PropelIQ-Team/.github/skills/artifact-resolver/assets/project-config.json', 'utf8'));
const a = cfg.artifacts.implement_task;
if (a) {
  console.log(JSON.stringify(a, null, 2));
} else {
  console.log('KEY NOT FOUND. Available keys:', Object.keys(cfg.artifacts).join(', '));
}
