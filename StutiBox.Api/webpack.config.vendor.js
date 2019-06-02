const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssNanoPlugin = require("cssnano");
const TerserWebpackPlugin = require("terser-webpack-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    var mode = isDevBuild ? "development" : "production";
    console.log('\x1b[36m%s\x1b[0m', "=== Webpack vendor compilation mode: " + mode + " ===");

    const sharedConfig = {
        mode,
        optimization: {
            minimize: !isDevBuild,
            usedExports: isDevBuild,
            minimizer: !isDevBuild ? [
                // Production.
                new TerserWebpackPlugin({
                    terserOptions: {
                        output: {
                            comments: false,
                        },
                    },
                }),
                new OptimizeCSSAssetsPlugin({
                    cssProcessor: CssNanoPlugin,
                    cssProcessorPluginOptions: {
                        preset: ["default", { discardComments: { removeAll: true } }]
                    }
                })
            ] : [
                // Development.
            ]
        },
        stats: { modules: false },
        resolve: { extensions: ['.js'] },
        module: {
            rules: [
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=100000' }
            ]
        },
        entry: {
            vendor: [
                'history',
                'react-router-dom',
                'react-router',
                'react-helmet',
                'react',
                'react-dom',
                'antd',
                'event-source-polyfill',
                '@babel/polyfill',
            ]
        },
        output: {
            publicPath: 'dist/',
            filename: '[name].js',
            library: '[name]_[hash]',
        },
        plugins: [
            new webpack.NormalModuleReplacementPlugin(/\/iconv-loader$/, require.resolve('node-noop')), // Workaround for https://github.com/andris9/encoding/issues/16
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
            })
        ].concat(isDevBuild ? [
            // Add module names to factory functions so they appear in browser profiler.
            new webpack.NamedModulesPlugin()
            ] : []),
        devtool: isDevBuild ? 'eval-source-map' : false,
        // If you have trouble with the vendor bundle in production mode 
        // you need to uncomment the line below and comment the line above.
        // Thus you will get an opportunity to have a full info about module 
        // in which the error occurs.
        //devtool: 'eval-source-map',
    };

    //start bundling client bundles
    const clientBundleOutputDir = path.join(__dirname, 'wwwroot', 'dist');
    console.log('\x1b[36m%s\x1b[0m', "*** Generating Client Bundles at: " + clientBundleOutputDir + " ***");

    const clientBundleConfig = merge(sharedConfig, {
        output: { path: clientBundleOutputDir },
        module: {
            rules: [
                { test: /\.css(\?|$)/, use: [MiniCssExtractPlugin.loader, 'css-loader'] }
            ]
        },
        plugins: [
            new MiniCssExtractPlugin({
                filename: "vendor.css"
            }),
            new webpack.DllPlugin({
                path: path.join(clientBundleOutputDir, '[name]-manifest.json'),
                name: '[name]_[hash]'
            })
        ].concat(isDevBuild ? [
            // Development.
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps.
                moduleFilenameTemplate: path.relative(clientBundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            }),
        ] : [
            // Production.
        ])
    });

    //todo: Add server bundle config as well: when SSR is ready
    return [clientBundleConfig]
}