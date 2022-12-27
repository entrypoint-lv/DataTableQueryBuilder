import { createWriteStream } from 'node:fs';
import { resolve } from 'node:path';
import { SitemapStream } from 'sitemap';

const SiteMapConfig = {
    hostname: 'https://entrypointdev.github.io/DataTableQueryBuilder/',
    links: []
};

export default {
    title: 'DataTable Query Builder',
    description: 'Server-side .NET query builder for JavaScript datatables',
    base: '/DataTableQueryBuilder/',

    themeConfig: {
        nav: [
            { text: 'Guide', link: '/guide/' },
        ],

        socialLinks: [
            { icon: 'github', link: 'https://github.com/EntryPointDev/DataTableQueryBuilder' },
        ],

        sidebar: [
            {
                text: 'Introduction',
                items: [
                    { text: 'Getting Started', link: '/guide/' },
                    { text: 'Basic Usage', link: '/guide/basic-usage.md' },
                    { text: 'How It Works', link: '/guide/how-it-works.md' },
                    { text: 'Value Matching', link: '/guide/value-matching.md' },
                    { text: 'Request Format', link: '/guide/request-format.md' },
                    
                ]
            },
            {
                text: 'Advanced',
                items: [
                    { text: 'Custom Expressions', link: '/guide/advanced/custom-search-sort.md' },
                    { text: 'Usage without Projection', link: '/guide/advanced/usage-without-projection.md' },
                ]
            },
            {
                text: 'Configuration',
                items: [
                    { text: 'Builder Options', link: '/guide/configuration/builder-options.md' },
                    { text: 'Field Options', link: '/guide/configuration/field-options.md' },
                ]
            },
            {
                text: 'Examples',
                items: [
                    { text: 'Demo and Samples', link: '/guide/samples.md' },
                ]
            }
        ],

        footer: {
            message: 'Released under the MIT License.',
            copyright: 'Copyright © 2019 - present <a href="http://entrypoint.lv">Entrypoint</a>'
        }
    },

    //sitemap generation
    transformHtml: (_, id, { pageData }) => {
        if (!/[\\/]404\.html$/.test(id))
            SiteMapConfig.links.push({
                // you might need to change this if not using clean urls mode
                url: pageData.relativePath.replace(/((^|\/)index)?\.md$/, '$2'),
                lastmod: pageData.lastUpdated
            })
    },

    buildEnd: async ({ outDir }) => {
        const sitemap = new SitemapStream({ hostname: SiteMapConfig.hostname });

        const writeStream = createWriteStream(resolve(outDir, 'sitemap.xml'));

        sitemap.pipe(writeStream);

        SiteMapConfig.links.forEach((link) => sitemap.write(link));

        sitemap.end();

        await new Promise((r) => writeStream.on('finish', r));
    }
}