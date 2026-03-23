'use client';

import React, { useState, useEffect, useCallback, useRef } from 'react';
import { motion } from 'framer-motion';
import { api } from '@/lib/api';
import { translations } from '@/lib/translations';
import Swal from 'sweetalert2';

// 1. Kategoriler ve API uç noktaları (İkonlar ve Renkler Eski Puk.html ile 1:1)
const slots = [
    { id: 'cpu', icon: 'memory', color: 'bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400', endpoint: 'processors' },
    { id: 'motherboard', icon: 'developer_board', color: 'bg-indigo-50 dark:bg-indigo-900/20 text-indigo-600 dark:text-indigo-400', endpoint: 'motherboards' },
    { id: 'ram', icon: 'straighten', color: 'bg-green-50 dark:bg-green-900/20 text-green-600 dark:text-green-400', endpoint: 'rams' },
    { id: 'gpu', icon: 'videogame_asset', color: 'bg-purple-50 dark:bg-purple-900/20 text-purple-600 dark:text-purple-400', endpoint: 'gpus' },
    { id: 'storage', icon: 'hard_drive', color: 'bg-orange-50 dark:bg-orange-900/20 text-orange-600 dark:text-orange-400', endpoint: 'storages' },
    { id: 'case', icon: 'computer', color: 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300', endpoint: 'cases' },
    { id: 'psu', icon: 'power', color: 'bg-yellow-50 dark:bg-yellow-900/20 text-yellow-600 dark:text-yellow-400', endpoint: 'psus' },
    { id: 'cpuCooler', icon: 'mode_fan', color: 'bg-cyan-50 dark:bg-cyan-900/20 text-cyan-600 dark:text-cyan-400', endpoint: 'cpucoolers' }
];

export default function PukPage() {
    const [lang, setLang] = useState('tr');
    const [selectedParts, setSelectedParts] = useState({
        cpu: null, motherboard: null, ram: null, gpu: null,
        storage: null, case: null, psu: null, cpuCooler: null
    });
    
    const [allData, setAllData] = useState({});
    const [loadingStates, setLoadingStates] = useState({});
    const [searchTerms, setSearchTerms] = useState({});
    const [openDropdown, setOpenDropdown] = useState(null);
    const [compatibilityResults, setCompatibilityResults] = useState({
        status: 'idle', // 'idle', 'success', 'fail'
        issues: []
    });

    // Refs
    const resultsRef = useRef(null);

    // Initial Lang
    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);
        
        const handleStorageChange = () => {
            setLang(localStorage.getItem('lang') || 'tr');
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, []);

    const t = translations[lang] || translations['tr'];

    // Data Loading
    const loadCategoryData = useCallback(async (catId, endpoint) => {
        if (allData[catId]) return;
        setLoadingStates(prev => ({ ...prev, [catId]: true }));
        try {
            const response = await api.get(endpoint);
            if (response.ok) {
                const data = await response.json();
                setAllData(prev => ({ ...prev, [catId]: data }));
            }
        } catch (error) {
            console.error(`Failed to load ${catId}:`, error);
        } finally {
            setLoadingStates(prev => ({ ...prev, [catId]: false }));
        }
    }, [allData]);

    const toggleDropdown = (catId, endpoint) => {
        if (openDropdown === catId) {
            setOpenDropdown(null);
        } else {
            setOpenDropdown(catId);
            loadCategoryData(catId, endpoint);
        }
    };

    const handleSelect = (catId, item) => {
        setSelectedParts(prev => ({ ...prev, [catId]: item }));
        setOpenDropdown(null);
        setSearchTerms(prev => ({ ...prev, [catId]: '' }));
        setCompatibilityResults({ status: 'idle', issues: [] });
    };

    const clearSelection = (e, catId) => {
        e.stopPropagation();
        setSelectedParts(prev => ({ ...prev, [catId]: null }));
        setCompatibilityResults({ status: 'idle', issues: [] });
    };

    // Close on outside click
    useEffect(() => {
        const handleClickOutside = (e) => {
            if (openDropdown && !e.target.closest('.custom-select-wrapper')) {
                setOpenDropdown(null);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [openDropdown]);

    // Compatibility Check (Ported from legacy)
    const checkCompatibility = () => {
        const activeCount = Object.values(selectedParts).filter(p => p !== null).length;
        if (activeCount < 2) {
            Swal.fire({
                title: lang === 'tr' ? 'Yetersiz Parça' : 'Insufficient Components',
                text: t['error-selection'],
                icon: 'warning',
                background: '#1f2937', color: '#fff', confirmButtonColor: '#16a3b2'
            });
            return;
        }

        const issues = [];
        const { cpu, motherboard: mobo, ram, gpu, psu, case: pcCase, cpuCooler: cooler } = selectedParts;

        // Simplified validation logic (consistent with Puk.html)
        const normalize = v => v ? String(v).trim().toLowerCase() : "";
        
        if (cpu && mobo) {
            if (normalize(cpu.socket) !== normalize(mobo.socket)) {
                issues.push({ level: 'critical', msg: `${lang === 'tr' ? 'İşlemci' : 'CPU'} soketi (${cpu.socket}) ile Anakart soketi (${mobo.socket}) uyumsuz.` });
            }
        }
        if (ram && mobo) {
            if (normalize(ram.memoryType) !== normalize(mobo.memoryType)) {
                issues.push({ level: 'critical', msg: `RAM tipi (${ram.memoryType}) ile Anakart (${mobo.memoryType}) uyumsuz.` });
            }
        }
        if (gpu && pcCase) {
            if (gpu.length > pcCase.maxGpuLength) {
                issues.push({ level: 'critical', msg: `Ekran kartı (${gpu.length}mm) kasaya sığmıyor (Max ${pcCase.maxGpuLength}mm).` });
            }
        }

        setCompatibilityResults({
            status: issues.length > 0 ? 'fail' : 'success',
            issues: issues
        });

        setTimeout(() => {
            resultsRef.current?.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }, 100);
    };

    const containerVariants = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: { staggerChildren: 0.05 }
        }
    };

    const itemVariants = {
        hidden: { opacity: 0, scale: 0.95 },
        visible: { opacity: 1, scale: 1, transition: { duration: 0.3 } }
    };

    return (
        <div className="relative min-h-screen py-12 md:py-20 overflow-hidden px-4 sm:px-6 lg:px-12 xl:px-24">
            <main className="w-full">
                
                {/* Header Section */}
                <div className="text-center max-w-4xl mx-auto mb-12">
                    <motion.h1 
                        initial={{ opacity: 0, y: -20 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="text-3xl md:text-5xl font-bold text-gray-900 dark:text-white tracking-tight mb-4"
                    >
                        {t["main-title"] || "PC Uyumluluk Kontrolü"}
                    </motion.h1>
                    <motion.p 
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ delay: 0.2 }}
                        className="text-lg text-gray-600 dark:text-gray-400"
                    >
                        {t["main-subtitle"] || "Sisteminizi aşağıdaki slotlara yerleştirin. Yapay zeka ve algoritmalarımız parçalarınızın uyumunu anında denetlesin."}
                    </motion.p>
                </div>

                {/* Slots Grid */}
                <motion.div 
                    variants={containerVariants}
                    initial="hidden"
                    animate="visible"
                    className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-12"
                >
                    {slots.map((slot) => (
                        <motion.div
                            key={slot.id}
                            variants={itemVariants}
                            className={`component-card group relative bg-white dark:bg-gray-800 rounded-xl border-2 transition-all duration-300 ${
                                selectedParts[slot.id] 
                                    ? 'border-primary shadow-lg' 
                                    : 'border-dashed border-gray-300 dark:border-gray-700 hover:border-primary/50'
                            }`}
                        >
                            <div className="flex items-center gap-4 mb-4 p-5">
                                <div className={`p-3 rounded-xl ${slot.color}`}>
                                    <span className="material-symbols-outlined text-2xl font-bold">{slot.icon}</span>
                                </div>
                                <h3 className="font-bold text-gray-900 dark:text-white text-lg">
                                    {t[`label-${slot.id}`] || t[`tab-${slot.id}`] || slot.id.toUpperCase()}
                                </h3>
                            </div>
                            
                            <div className="px-5 pb-6">
                                <div className="custom-select-wrapper group/select relative">
                                    <div 
                                        className={`custom-select-trigger w-full flex items-center justify-between border rounded-lg px-4 py-3 cursor-pointer transition-all ${
                                            openDropdown === slot.id 
                                                ? 'border-primary ring-2 ring-primary/10' 
                                                : 'border-gray-200 dark:border-gray-700 dark:bg-gray-900/50'
                                        }`}
                                        onClick={() => toggleDropdown(slot.id, slot.endpoint)}
                                    >
                                        <span className={`truncate select-none font-medium ${selectedParts[slot.id] ? 'text-gray-900 dark:text-white' : 'text-gray-400'}`}>
                                            {selectedParts[slot.id] ? `${selectedParts[slot.id].brand} ${selectedParts[slot.id].modelName}` : t["select-default"]}
                                        </span>
                                        <div className="flex items-center gap-1">
                                            {selectedParts[slot.id] && (
                                                <button onClick={(e) => clearSelection(e, slot.id)} className="text-gray-400 hover:text-red-500 transition-colors">
                                                    <span className="material-symbols-outlined text-lg">close</span>
                                                </button>
                                            )}
                                            <span className={`material-symbols-outlined text-gray-400 transition-transform ${openDropdown === slot.id ? 'rotate-180' : ''}`}>expand_more</span>
                                        </div>
                                    </div>

                                    {openDropdown === slot.id && (
                                        <div className="absolute left-0 right-0 top-full mt-2 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl shadow-2xl z-[100] overflow-hidden animate-fade-in-down">
                                            <div className="p-3 border-b border-gray-100 dark:border-gray-700">
                                                <input 
                                                    type="text"
                                                    placeholder={t['search-placeholder'] || "Ara..."}
                                                    className="w-full bg-gray-50 dark:bg-gray-900 border border-gray-200 dark:border-gray-700 rounded-lg px-3 py-2 text-sm focus:border-primary outline-none"
                                                    value={searchTerms[slot.id] || ''}
                                                    onChange={(e) => setSearchTerms(prev => ({ ...prev, [slot.id]: e.target.value }))}
                                                    autoFocus
                                                />
                                            </div>
                                            <div className="max-h-60 overflow-y-auto custom-scrollbar">
                                                {loadingStates[slot.id] ? (
                                                    <div className="p-6 text-center text-gray-400 text-sm animate-pulse">{t['loading-data']}</div>
                                                ) : (
                                                    allData[slot.id]?.filter(item => {
                                                        const search = (searchTerms[slot.id] || '').toLowerCase();
                                                        return `${item.brand} ${item.modelName}`.toLowerCase().includes(search);
                                                    }).map(item => (
                                                        <div 
                                                            key={item.id}
                                                            className="px-4 py-3 text-sm text-gray-700 dark:text-gray-300 hover:bg-primary/10 hover:text-primary cursor-pointer transition-colors"
                                                            onClick={() => handleSelect(slot.id, item)}
                                                        >
                                                            {item.brand} {item.modelName}
                                                        </div>
                                                    ))
                                                )}
                                            </div>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </motion.div>
                    ))}
                </motion.div>

                {/* Results Section */}
                <div className="max-w-5xl mx-auto space-y-12" ref={resultsRef}>
                    <div className="flex flex-col md:flex-row items-center justify-center gap-4">
                        <button 
                            onClick={checkCompatibility}
                            className="w-full md:w-auto bg-primary text-white font-black py-4 px-12 rounded-xl text-lg hover:bg-primary/90 hover:scale-105 transition-all shadow-xl shadow-primary/25"
                        >
                            {t["action-check"]}
                        </button>
                        <button className="w-full md:w-auto bg-indigo-600 text-white font-black py-4 px-12 rounded-xl text-lg hover:bg-indigo-700 hover:scale-105 transition-all shadow-xl shadow-indigo-600/30 opacity-50 cursor-not-allowed">
                            {t["action-expert"]} ✨
                        </button>
                    </div>

                    <div className="bg-white dark:bg-gray-800/80 backdrop-blur-xl p-8 rounded-3xl border border-gray-200 dark:border-gray-700 shadow-2xl">
                        <div className="flex items-center gap-4 mb-8 border-b border-gray-100 dark:border-gray-800 pb-6">
                            <div className="p-3 bg-primary/10 rounded-xl">
                                <span className="material-symbols-outlined text-primary text-3xl">analytics</span>
                            </div>
                            <h2 className="text-2xl font-black text-gray-900 dark:text-white italic tracking-tight">
                                {t["results-title"]}
                            </h2>
                        </div>

                        {compatibilityResults.status === 'idle' && (
                            <div className="flex flex-col md:flex-row items-center gap-6 p-8 rounded-2xl bg-blue-50/50 dark:bg-blue-900/10 border border-blue-100/50 dark:border-blue-900/20">
                                <div className="p-5 bg-blue-100 dark:bg-blue-600/20 rounded-full animate-bounce">
                                    <span className="material-symbols-outlined text-4xl text-blue-600 dark:text-blue-400">info</span>
                                </div>
                                <div className="text-center md:text-left">
                                    <h3 className="text-xl font-bold text-blue-900 dark:text-blue-100 mb-2">{t["result-info-title"]}</h3>
                                    <p className="text-blue-700 dark:text-blue-300/80 text-base font-medium">{t["result-info-text"]}</p>
                                </div>
                            </div>
                        )}

                        {compatibilityResults.status === 'success' && (
                            <div className="flex flex-col md:flex-row items-center gap-6 p-8 rounded-2xl bg-emerald-50 dark:bg-emerald-950/20 border border-emerald-100 dark:border-emerald-900/30">
                                <div className="p-5 bg-emerald-500 rounded-full shadow-lg shadow-emerald-500/20">
                                    <span className="material-symbols-outlined text-4xl text-white">verified</span>
                                </div>
                                <div className="text-center md:text-left">
                                    <h3 className="text-xl font-bold text-emerald-900 dark:text-emerald-100 mb-2">{t["result-success-title"] || "Mükemmel Uyum!"}</h3>
                                    <p className="text-emerald-700 dark:text-emerald-400 font-medium">Tüm parçalar birbiriyle uyumlu çalışacaktır. Kurulumu gerçekleştirebilirsiniz.</p>
                                </div>
                            </div>
                        )}

                        {compatibilityResults.status === 'fail' && (
                            <div className="space-y-4">
                                <div className="flex items-center gap-3 text-red-600 dark:text-red-400 mb-4 px-2">
                                    <span className="material-symbols-outlined">warning</span>
                                    <h3 className="text-lg font-bold">{t["result-fail-title"]}</h3>
                                </div>
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    {compatibilityResults.issues.map((issue, idx) => (
                                        <div key={idx} className="flex items-start gap-4 p-5 rounded-2xl bg-red-50 dark:bg-red-950/10 border border-red-100 dark:border-red-900/20">
                                            <span className="material-symbols-outlined text-red-500 mt-0.5">error</span>
                                            <p className="text-red-900 dark:text-red-200 text-sm font-medium leading-relaxed">{issue.msg}</p>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </main>
        </div>
    );
}
