import{_ as a,c as s,o as e,a as t}from"./app.9dbf2506.js";const b=JSON.parse('{"title":"Server-side .NET query builder for JavaScript data tables","description":"","frontmatter":{},"headers":[{"level":2,"title":"Usage with DataTables","slug":"usage-with-datatables","link":"#usage-with-datatables","children":[]},{"level":2,"title":"Usage with other JavaScript datatables","slug":"usage-with-other-javascript-datatables","link":"#usage-with-other-javascript-datatables","children":[]}],"relativePath":"guide/index.md"}'),n={name:"guide/index.md"},l=t(`<h1 id="server-side-net-query-builder-for-javascript-data-tables" tabindex="-1">Server-side .NET query builder for JavaScript data tables <a class="header-anchor" href="#server-side-net-query-builder-for-javascript-data-tables" aria-hidden="true">#</a></h1><p>This builder automatically transforms AJAX requests coming from a JavaScript datatable into LINQ queries against the Entity Framework data model according to the provided configuration.</p><p>Can be used with ANY JavaScript datatable component that supports server-side processing (currently tested on <a href="https://datatables.net" target="_blank" rel="noreferrer">datatables.net</a> and <a href="https://xaksis.github.io/vue-good-table/" target="_blank" rel="noreferrer">vue-good-table</a> only).</p><h2 id="usage-with-datatables" tabindex="-1">Usage with DataTables <a class="header-anchor" href="#usage-with-datatables" aria-hidden="true">#</a></h2><p>If you&#39;re using <a href="https://datatables.net" target="_blank" rel="noreferrer">DataTables</a> or wrappers around it, install the <a href="https://www.nuget.org/packages/datatablequerybuilder.datatables/" target="_blank" rel="noreferrer">DataTableQueryBuilder.DataTables</a> NuGet package:</p><div class="language-shell"><button title="Copy Code" class="copy"></button><span class="lang">shell</span><pre class="shiki material-palenight"><code><span class="line"><span style="color:#FFCB6B;">dotnet</span><span style="color:#A6ACCD;"> </span><span style="color:#C3E88D;">add</span><span style="color:#A6ACCD;"> </span><span style="color:#C3E88D;">package</span><span style="color:#A6ACCD;"> </span><span style="color:#C3E88D;">DataTableQueryBuilder.DataTables</span></span>
<span class="line"></span></code></pre></div><p>Then register the model binder to bind incoming AJAX requests from DataTables to a <code>DataTableRequest</code> model:</p><div class="language-c#"><button title="Copy Code" class="copy"></button><span class="lang">c#</span><pre class="shiki material-palenight"><code><span class="line"><span style="color:#F78C6C;">using</span><span style="color:#A6ACCD;"> DataTableQueryBuilder.DataTables</span><span style="color:#89DDFF;">;</span></span>
<span class="line"></span>
<span class="line"><span style="color:#C792EA;">public</span><span style="color:#A6ACCD;"> </span><span style="color:#F78C6C;">class</span><span style="color:#A6ACCD;"> </span><span style="color:#FFCB6B;">Startup</span></span>
<span class="line"><span style="color:#89DDFF;">{</span></span>
<span class="line"><span style="color:#89DDFF;">    </span><span style="color:#676E95;font-style:italic;">//...</span></span>
<span class="line"></span>
<span class="line"><span style="color:#A6ACCD;">    </span><span style="color:#C792EA;">public</span><span style="color:#A6ACCD;"> </span><span style="color:#89DDFF;">void</span><span style="color:#A6ACCD;"> </span><span style="color:#82AAFF;">ConfigureServices</span><span style="color:#89DDFF;">(</span><span style="color:#FFCB6B;">IServiceCollection</span><span style="color:#A6ACCD;"> </span><span style="color:#FFCB6B;">services</span><span style="color:#89DDFF;">)</span></span>
<span class="line"><span style="color:#A6ACCD;">    </span><span style="color:#89DDFF;">{</span></span>
<span class="line"><span style="color:#89DDFF;">        </span><span style="color:#676E95;font-style:italic;">//...</span></span>
<span class="line"><span style="color:#A6ACCD;">        </span></span>
<span class="line"><span style="color:#A6ACCD;">        services</span><span style="color:#89DDFF;">.</span><span style="color:#82AAFF;">RegisterDataTables</span><span style="color:#89DDFF;">();</span></span>
<span class="line"><span style="color:#A6ACCD;">    </span><span style="color:#89DDFF;">}</span><span style="color:#A6ACCD;">   </span></span>
<span class="line"><span style="color:#89DDFF;">}</span></span>
<span class="line"></span></code></pre></div><h2 id="usage-with-other-javascript-datatables" tabindex="-1">Usage with other JavaScript datatables <a class="header-anchor" href="#usage-with-other-javascript-datatables" aria-hidden="true">#</a></h2><p>For other JavaScript datatable components, install the <a href="https://www.nuget.org/packages/datatablequerybuilder.generic/" target="_blank" rel="noreferrer">DataTableQueryBuilder.Generic</a> NuGet package instead:</p><div class="language-shell"><button title="Copy Code" class="copy"></button><span class="lang">shell</span><pre class="shiki material-palenight"><code><span class="line"><span style="color:#FFCB6B;">dotnet</span><span style="color:#A6ACCD;"> </span><span style="color:#C3E88D;">add</span><span style="color:#A6ACCD;"> </span><span style="color:#C3E88D;">package</span><span style="color:#A6ACCD;"> </span><span style="color:#C3E88D;">DataTableQueryBuilder.Generic</span></span>
<span class="line"></span></code></pre></div><p>Nothing else is needed.</p>`,12),r=[l];function p(o,i,c,d,u,h){return e(),s("div",null,r)}const C=a(n,[["render",p]]);export{b as __pageData,C as default};