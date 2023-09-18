const { createProxyMiddleware } = require('http-proxy-middleware');
module.exports = function (app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: process.env.proxy_url,
      changeOrigin: true,
      // pathRewrite: {
      //   '^/api': ''
      // }
    })
  );
};