'use client';

import React, { useState, useEffect, useMemo, useCallback } from 'react';
import { motion } from 'framer-motion';
import { api } from '@/lib/api';
import { translations } from '@/lib/translations';
import Swal from 'sweetalert2';

// 1. API uç noktaları (pts.html ile 1:1)
const COMPONENT_ENDPOINTS = {
    cpu: 'processors',
    motherboard: 'motherboards',
    ram: 'rams',
    gpu: 'gpus',
    storage: 'storages',
    case: 'cases',
    psu: 'psus',
    cpuCooler: 'cpuCoolers'
};

const ITEMS_PER_PAGE = 24;

export default function PtsPage() {
    const [lang, setLang] = useState('tr');
    const [currentCategory, setCurrentCategory] = useState('cpu');
    const [allData, setAllData] = useState({
        cpu: [], motherboard: [], ram: [], gpu: [],
        storage: [], case: [], psu: [], cpuCooler: []
    });
    const [currentBuild, setCurrentBuild] = useState({
        cpu: null, motherboard: null, ram: null, gpu: null,
        storage: null, case: null, psu: null, cpuCooler: null
    });
    const [loading, setLoading] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const [searchQuery, setSearchQuery] = useState('');
    const [selectedFilters, setSelectedFilters] = useState({});
    const [expandedFilters, setExpandedFilters] = useState({});
    const [filterShowMore, setFilterShowMore] = useState({});

    const t = translations[lang] || translations['tr'];

    // Dil ve İlk Yükleme
    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);
        
        const handleStorageChange = () => {
            setLang(localStorage.getItem('lang') || 'tr');
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, []);

    // Veri Çekme (Lazy Load + Cache)
    useEffect(() => {
        const fetchData = async () => {
            if (allData[currentCategory]?.length > 0) return;
            setLoading(true);
            try {
                const endpoint = COMPONENT_ENDPOINTS[currentCategory];
                const response = await api.get(endpoint);
                if (response.ok) {
                    const data = await response.json();
                    if (Array.isArray(data)) {
                        setAllData(prev => ({ ...prev, [currentCategory]: data }));
                    }
                }
            } catch (error) {
                console.error("Fetch error:", error);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
        setCurrentPage(1);
        setSearchQuery('');
        setSelectedFilters({});
    }, [currentCategory, allData]);

    // Uyumsuzluk Kontrolü (Legacy pts.html logic)
    const compatibilityInfo = useMemo(() => {
        const messages = {};
        const items = allData[currentCategory] || [];
        
        items.forEach(item => {
            let failReason = "";
            let isCompatible = true;

            const normalize = v => v ? String(v).trim().toLowerCase() : "";

            if (currentCategory === 'cpu' && currentBuild.motherboard) {
                if (normalize(item.socket) !== normalize(currentBuild.motherboard.socket)) {
                    isCompatible = false;
                    failReason = t["incompatible-socket-cpu"];
                }
            }

            if (currentCategory === 'motherboard') {
                if (currentBuild.cpu && normalize(item.socket) !== normalize(currentBuild.cpu.socket)) {
                    isCompatible = false;
                    failReason = t["incompatible-socket-mobo"];
                }
                if (currentBuild.ram && normalize(item.memoryType) !== normalize(currentBuild.ram.memoryType)) {
                    isCompatible = false;
                    failReason = t["incompatible-ramType"];
                }
            }

            if (currentCategory === 'ram' && currentBuild.motherboard) {
                if (normalize(item.memoryType) !== normalize(currentBuild.motherboard.memoryType)) {
                    isCompatible = false;
                    failReason = t["incompatible-ramType"];
                }
            }

            if (!isCompatible) messages[item.id] = failReason;
        });
        return messages;
    }, [allData, currentCategory, currentBuild, t]);

    // Filtreleme ve Arama
    const filteredItems = useMemo(() => {
        let items = [...(allData[currentCategory] || [])];

        if (searchQuery) {
            const lowQuery = searchQuery.toLowerCase();
            items = items.filter(item => 
                (item.modelName || "").toLowerCase().includes(lowQuery) || 
                (item.brand || "").toLowerCase().includes(lowQuery)
            );
        }

        // Kategorik filtreler
        for (const [type, values] of Object.entries(selectedFilters)) {
            if (values.length > 0) {
                items = items.filter(item => values.includes(String(item[type])));
            }
        }

        // Uyumlu olanları öne çıkar
        items.sort((a, b) => {
            const isAInc = !!compatibilityInfo[a.id] ? 1 : 0;
            const isBInc = !!compatibilityInfo[b.id] ? 1 : 0;
            return isAInc - isBInc;
        });

        return items;
    }, [allData, currentCategory, searchQuery, selectedFilters, compatibilityInfo]);

    const paginatedItems = filteredItems.slice((currentPage - 1) * ITEMS_PER_PAGE, currentPage * ITEMS_PER_PAGE);
    const totalPages = Math.ceil(filteredItems.length / ITEMS_PER_PAGE);

    // Hesaplamalar
    const totals = useMemo(() => {
        const cpuTdp = currentBuild.cpu?.tdp || 0;
        const gpuTdp = currentBuild.gpu?.tdp || 0;
        const wattage = cpuTdp + gpuTdp;
        const price = Object.values(currentBuild).reduce((sum, item) => sum + (item?.price || 0), 0);
        const psu = Math.max(300, Math.ceil(((wattage + 100) * 1.25) / 50) * 50);
        return { wattage, psu, price };
    }, [currentBuild]);

    const handleCategoryChange = (cat) => setCurrentCategory(cat);
    const addToBuild = (item) => setCurrentBuild(prev => ({ ...prev, [currentCategory]: item }));
    const removeFromBuild = (type) => setCurrentBuild(prev => ({ ...prev, [type]: null }));

    const handleFilterChange = (type, value) => {
        setSelectedFilters(prev => {
            const current = prev[type] || [];
            if (current.includes(value)) return { ...prev, [type]: current.filter(v => v !== value) };
            return { ...prev, [type]: [...current, value] };
        });
        setCurrentPage(1);
    };

    // Dinamik Filtre Özellikleri
    const dynamicFilterProps = useMemo(() => {
        const configMap = {
            cpu: ['brand', 'socket', 'coreCount'],
            motherboard: ['brand', 'socket', 'chipset', 'formFactor', 'memoryType'],
            ram: ['brand', 'memoryType', 'capacityPerModule', 'speed'],
            gpu: ['brand', 'chipset'],
            case: ['brand', 'caseType'],
            psu: ['brand', 'wattage', 'rating'],
            storage: ['brand', 'storageType', 'capacity'],
            cpuCooler: ['brand', 'coolerType']
        };
        const props = configMap[currentCategory] || [];
        const items = allData[currentCategory] || [];

        return props.map(prop => {
            const uniqueValues = [...new Set(items.map(i => String(i[prop])))].filter(v => v && v !== 'null').sort();
            return { prop, values: uniqueValues };
        }).filter(f => f.values.length > 0);
    }, [currentCategory, allData]);

    return (
        <div className="flex flex-col flex-1 bg-background-light dark:bg-background-dark min-h-screen pt-24 lg:pt-28">
            
            {/* Premium Segmented Control - SCROLLABLE (Un-pinned for better UX) */}
            <div className="relative w-full bg-[#111c22]/90 backdrop-blur-md border-b border-white/5 shadow-2xl overflow-hidden z-20">
                <div className="w-full h-16 flex items-center px-4 sm:px-6 lg:px-12 xl:px-24 overflow-x-auto no-scrollbar scroll-smooth">
                    <div className="flex h-full min-w-max">
                        {Object.keys(COMPONENT_ENDPOINTS).map(cat => {
                            const isSelected = !!currentBuild[cat];
                            const isActive = currentCategory === cat;
                            return (
                                <button
                                    key={cat}
                                    onClick={() => handleCategoryChange(cat)}
                                    className={`relative flex items-center gap-3 h-full px-8 text-[11px] font-black uppercase tracking-[0.2em] transition-all duration-300 group
                                        ${isActive 
                                            ? 'text-primary bg-primary/5 after:content-[""] after:absolute after:bottom-0 after:left-0 after:w-full after:h-[3px] after:bg-primary after:shadow-[0_-4px_12px_rgba(22,163,178,0.5)]' 
                                            : isSelected 
                                                ? 'text-green-500/80 hover:text-green-400 hover:bg-white/5' 
                                                : 'text-white/40 hover:text-white hover:bg-white/5'
                                        }
                                    `}
                                >
                                    <div className="relative flex items-center gap-2.5">
                                        <span className={`material-symbols-outlined text-lg transition-transform duration-300 group-hover:scale-110 ${isActive ? 'scale-110' : ''}`}>
                                            {getIcon(cat)}
                                        </span>
                                        <span>{t[`tab-${cat === 'motherboard' ? 'mobo' : cat === 'cpuCooler' ? 'cooler' : cat}`]}</span>
                                        
                                        {/* Completed Indicator - Minimalist Checkmark */}
                                        {isSelected && !isActive && (
                                            <motion.span 
                                                initial={{ scale: 0, opacity: 0 }}
                                                animate={{ scale: 1, opacity: 1 }}
                                                className="material-symbols-outlined text-sm text-green-500"
                                            >
                                                check_circle
                                            </motion.span>
                                        )}
                                    </div>
                                </button>
                            );
                        })}
                    </div>
                </div>
            </div>

            <main className="w-full py-8 px-4 sm:px-6 lg:px-12 xl:px-24">
                <div className="flex flex-col lg:flex-row gap-8">
                    
                    {/* Left Sidebar: Filters - STICKY PINNED */}
                    <aside className="w-full lg:w-1/4">
                        <div className="sticky top-24 h-fit max-h-[calc(100vh-120px)] overflow-y-auto custom-scrollbar bg-white dark:bg-card-darker rounded-xl border border-gray-200 dark:border-gray-800 shadow-xl">
                            <div className="p-5 border-b border-gray-100 dark:border-gray-800">
                                <h3 className="text-sm font-black uppercase tracking-widest text-gray-900 dark:text-white mb-4">{t["filter-title"]}</h3>
                                <div className="relative">
                                    <input 
                                        type="text" 
                                        value={searchQuery}
                                        onChange={(e) => setSearchQuery(e.target.value)}
                                        placeholder={t["search-placeholder"]}
                                        className="w-full bg-gray-50 dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-3 text-sm focus:border-primary outline-none transition-all"
                                    />
                                </div>
                            </div>
                            <div className="p-4 max-h-[60vh] overflow-y-auto custom-scrollbar">
                                {dynamicFilterProps.map(({ prop, values }) => (
                                    <div key={prop} className="border-b border-gray-50 dark:border-gray-800 last:border-0 py-2">
                                        <button 
                                            className="w-full flex justify-between items-center py-2 text-left group"
                                            onClick={() => setExpandedFilters(p => ({...p, [prop]: !p[prop]}))}
                                        >
                                            <span className="text-xs font-bold text-gray-500 dark:text-gray-400 group-hover:text-primary uppercase">{t[`filter-${prop}`] || prop}</span>
                                            <span className={`material-symbols-outlined text-gray-400 transition-transform ${expandedFilters[prop] ? 'rotate-180' : ''}`}>expand_more</span>
                                        </button>
                                        {expandedFilters[prop] && (
                                            <div className="space-y-1 mt-1 pb-3">
                                                {(filterShowMore[prop] ? values : values.slice(0, 5)).map(v => (
                                                    <label key={v} className="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-50 dark:hover:bg-white/5 cursor-pointer">
                                                        <input 
                                                            type="checkbox" 
                                                            className="rounded border-gray-300 dark:border-gray-700 text-primary focus:ring-primary h-4 w-4"
                                                            checked={(selectedFilters[prop] || []).includes(v)}
                                                            onChange={() => handleFilterChange(prop, v)}
                                                        />
                                                        <span className="text-sm text-gray-700 dark:text-gray-300 font-medium">{v === 'true' ? t['filter-true'] : v === 'false' ? t['filter-false'] : v}</span>
                                                    </label>
                                                ))}
                                                {values.length > 5 && (
                                                    <button onClick={() => setFilterShowMore(p => ({...p, [prop]: !p[prop]}))} className="text-[10px] font-black text-primary px-2 uppercase mt-1">
                                                        {filterShowMore[prop] ? t["action-show-less"] : t["action-show-all"]}
                                                    </button>
                                                )}
                                            </div>
                                        )}
                                    </div>
                                ))}
                            </div>
                        </div>
                    </aside>

                    {/* Middle: Component List */}
                    <div className="w-full lg:w-2/4">
                        <div className="flex justify-between items-end mb-6 bg-gradient-to-r from-primary/10 to-transparent p-5 rounded-xl border-l-4 border-primary">
                            <div>
                                <h2 className="text-xl font-bold text-gray-900 dark:text-white uppercase tracking-tight">{t[`step-title-${currentCategory === 'motherboard' ? 'motherboard' : currentCategory}`]}</h2>
                                <p className="text-[10px] font-black text-gray-500 mt-1 uppercase tracking-[0.2em]">{filteredItems.length} {lang === 'tr' ? 'Parça Listeleniyor' : 'Components Found'}</p>
                            </div>
                        </div>

                        <div className="space-y-4 min-h-[500px]">
                            {loading ? (
                                <div className="text-center py-20 animate-pulse">
                                    <span className="material-symbols-outlined text-5xl text-primary animate-spin">sync</span>
                                    <p className="mt-4 font-bold text-gray-400 uppercase tracking-widest">{lang === 'tr' ? 'Veritabanı Okunuyor...' : 'Reading Database...'}</p>
                                </div>
                            ) : paginatedItems.map(item => {
                                const isSelected = currentBuild[currentCategory]?.id === item.id;
                                const inc = compatibilityInfo[item.id] && !isSelected;
                                return (
                                    <div key={item.id} className={`group bg-white dark:bg-card-dark p-4 rounded-xl border transition-all duration-300 flex gap-6 ${isSelected ? 'border-primary shadow-2xl bg-primary/5' : 'border-gray-200 dark:border-gray-700 hover:border-primary/40'} ${inc ? 'opacity-60 saturate-50' : ''}`}>
                                        <div className="w-32 h-32 bg-gray-50 dark:bg-gray-800/50 rounded-xl flex items-center justify-center p-3 shrink-0 border border-transparent group-hover:border-primary/10 transition-colors">
                                            <img src={item.imageUrl || `https://placehold.co/100?text=${item.brand}`} className="max-h-full object-contain" alt="" />
                                        </div>
                                        <div className="flex-grow flex flex-col justify-between py-1">
                                            <div>
                                                <h4 className="text-lg font-black text-gray-900 dark:text-white leading-tight uppercase tracking-tight">{item.brand} {item.modelName}</h4>
                                                <div className="mt-2 grid grid-cols-2 gap-x-4 gap-y-1 text-[11px] font-bold text-gray-500 uppercase">
                                                    {renderSpecs(item, currentCategory, lang)}
                                                </div>
                                                {inc && (
                                                    <div className="mt-3 flex items-center gap-2 text-red-600 bg-red-100/50 dark:bg-red-500/10 px-3 py-1.5 rounded-lg border border-red-200 dark:border-red-900/50">
                                                        <span className="material-symbols-outlined text-sm">report_problem</span>
                                                        <span className="text-[10px] font-black tracking-wide">{compatibilityInfo[item.id]}</span>
                                                    </div>
                                                )}
                                            </div>
                                            <div className="flex items-center justify-between border-t border-gray-100 dark:border-gray-800 pt-3 mt-3">
                                                <span className="text-xl font-bold text-primary group-hover:scale-105 transition-transform origin-left">{(item.price || 0).toLocaleString()} TL</span>
                                                <button 
                                                    onClick={() => addToBuild(item)}
                                                    disabled={isSelected}
                                                    className={`px-6 py-2 rounded-lg font-bold text-[10px] uppercase tracking-widest transition-all ${isSelected ? 'bg-green-600 text-white' : 'bg-primary text-white hover:brightness-110 active:scale-95 shadow-lg shadow-primary/20'}`}
                                                >
                                                    {isSelected ? t["action-selected"] : t["action-add"]}
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                );
                            })}
                        </div>

                        {totalPages > 1 && (
                            <div className="mt-10 flex justify-center gap-2">
                                <button onClick={() => setCurrentPage(p => Math.max(1, p-1))} className="w-10 h-10 rounded-lg border dark:border-gray-700 hover:border-primary transition-colors flex items-center justify-center"><span className="material-symbols-outlined">chevron_left</span></button>
                                <span className="flex items-center px-4 font-bold text-gray-500 text-sm whitespace-nowrap">{currentPage} / {totalPages}</span>
                                <button onClick={() => setCurrentPage(p => Math.min(totalPages, p+1))} className="w-10 h-10 rounded-lg border dark:border-gray-700 hover:border-primary transition-colors flex items-center justify-center"><span className="material-symbols-outlined">chevron_right</span></button>
                            </div>
                        )}
                    </div>

                    {/* Right Sidebar: Summary - STICKY PINNED */}
                    <aside className="w-full lg:w-1/4">
                        <div className="sticky top-24 h-fit bg-white dark:bg-card-darker rounded-xl border border-gray-200 dark:border-gray-800 shadow-2xl flex flex-col">
                            <div className="p-5 border-b border-gray-100 dark:border-gray-800 bg-gray-50/50 dark:bg-white/5">
                                <h3 className="text-sm font-black uppercase tracking-[0.2em] flex items-center gap-3">
                                    <span className="material-symbols-outlined text-primary">inventory_2</span>
                                    {t["System-summary"]}
                                </h3>
                            </div>
                            <div className="p-4 space-y-3 max-h-[45vh] overflow-y-auto custom-scrollbar">
                                {Object.entries(currentBuild).map(([type, item]) => item && (
                                    <div key={type} className="flex items-center gap-3 p-2 bg-gray-50 dark:bg-gray-800/40 rounded-xl border border-transparent hover:border-primary/20 transition-all">
                                        <div className="w-12 h-12 bg-white dark:bg-gray-900 border dark:border-gray-800 rounded-lg p-1.5 shrink-0">
                                            <img src={item.imageUrl} className="w-full h-full object-contain" alt="" />
                                        </div>
                                        <div className="flex-grow min-w-0">
                                            <span className="text-[9px] font-black text-primary uppercase tracking-widest block">{t[`tab-${type === 'motherboard' ? 'mobo' : type === 'cpuCooler' ? 'cooler' : type}`]}</span>
                                            <p className="text-[11px] font-bold truncate pr-1">{item.modelName}</p>
                                        </div>
                                        <button onClick={() => removeFromBuild(type)} className="text-gray-400 hover:text-red-500 transition-colors"><span className="material-symbols-outlined text-lg">close</span></button>
                                    </div>
                                ))}
                                {Object.values(currentBuild).every(v => v === null) && (
                                    <div className="text-center py-10 opacity-20">
                                        <span className="material-symbols-outlined text-4xl mb-2">add_shopping_cart</span>
                                        <p className="text-[10px] font-black uppercase tracking-widest">{t["msg-no-parts"]}</p>
                                    </div>
                                )}
                            </div>
                            <div className="p-6 bg-gray-50 dark:bg-white/5 border-t border-gray-100 dark:border-gray-800 space-y-6">
                                <div className="space-y-2 text-[10px] font-black uppercase tracking-widest text-gray-500">
                                    <div className="flex justify-between"><span>{t["summary-estimated"]}</span><span className="text-gray-900 dark:text-white">{totals.wattage} W</span></div>
                                    <div className="flex justify-between"><span>{t["summary-recommended"]}</span><span className="text-green-600">{totals.psu} W</span></div>
                                </div>
                                <div className="border-t border-gray-200 dark:border-gray-700 pt-4">
                                    <div className="flex justify-between items-baseline mb-6">
                                        <span className="text-[10px] font-black uppercase tracking-[0.2em]">{t["summary-total"]}</span>
                                        <span className="text-2xl font-bold text-primary">{(totals.price || 0).toLocaleString()} TL</span>
                                    </div>
                                    <button onClick={() => Swal.fire('Tebrikler!', 'Sistem Toplama Başarıyla Tamamlandı.', 'success')} className="w-full bg-primary hover:brightness-110 text-white font-black py-4 rounded-xl shadow-xl shadow-primary/20 transition-all uppercase tracking-widest text-xs flex items-center justify-center gap-3 active:scale-95">
                                        <span className="material-symbols-outlined text-lg">save_as</span>
                                        {t["action-save-system"]}
                                    </button>
                                </div>
                            </div>
                        </div>
                    </aside>

                </div>
            </main>
        </div>
    );
}

function getIcon(cat) {
    const icons = { cpu:'memory', motherboard:'developer_board', ram:'straighten', gpu:'videogame_asset', storage:'sd_storage', case:'computer', psu:'power', cpuCooler:'mode_fan' };
    return icons[cat] || 'build';
}

function renderSpecs(item, category, lang) {
    const TR = lang === 'tr';
    switch(category) {
        case 'cpu': return <><p>{TR?'Soket':'Socket'}: {item.socket}</p><p>{TR?'Çekirdek':'Cores'}: {item.coreCount}</p></>;
        case 'motherboard': return <><p>{TR?'Soket':'Socket'}: {item.socket}</p><p>RAM: {item.memoryType}</p></>;
        case 'ram': return <><p>TIP: {item.memoryType}</p><p>HIZ: {item.speed}MHz</p></>;
        case 'gpu': return <><p>VRAM: {item.vramMemorySize}GB</p><p> TDP: {item.tdp}W</p></>;
        case 'case': return <><p>TIP: {item.caseType}</p><p>GPU: {item.maxGpuLength}mm</p></>;
        case 'psu': return <><p>WATT: {item.wattage}W</p><p>VERIM: {item.rating}</p></>;
        case 'storage': return <><p>KAP: {item.capacity}GB</p><p>TIP: {item.storageType}</p></>;
        case 'cpuCooler': return <><p>TIP: {item.coolerType}</p><p>RAD: {item.radiatorSize ? `${item.radiatorSize}mm` : 'N/A'}</p></>;
        default: return null;
    }
}
