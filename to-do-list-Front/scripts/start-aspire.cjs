const { spawn } = require('node:child_process');

const port = process.env.PORT ?? '4300';

const child = spawn(
  process.execPath,
  [
    require.resolve('@angular/cli/bin/ng.js'),
    'serve',
    '--proxy-config',
    'proxy.conf.json',
    '--port',
    port,
  ],
  {
    stdio: 'inherit',
    env: process.env,
  },
);

child.on('exit', (code, signal) => {
  if (signal) {
    process.kill(process.pid, signal);
    return;
  }

  process.exit(code ?? 0);
});
