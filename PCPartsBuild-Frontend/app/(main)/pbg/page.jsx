"use client";
import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { translations } from '@/lib/translations';

export default function PBGPage() {
    const [lang, setLang] = useState('tr');
    
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

    const categories = [
        { id: 'cpu', icon: '/Images/cpu.webp', imgClass: 'h-40' },
        { id: 'gpu', icon: '/Images/ekrankarti.webp', imgClass: 'h-32' },
        { id: 'ram', icon: '/Images/ram.webp', imgClass: 'h-32' },
        { id: 'storage', icon: '/Images/ssd.webp', imgClass: 'h-32' },
        { id: 'motherboard', icon: '/Images/anakart.webp', imgClass: 'h-40' },
        { id: 'psu', icon: '/Images/guc.webp', imgClass: 'h-40' },
        { id: 'case', icon: '/Images/kasa.webp', imgClass: 'h-40' },
        { id: 'cpuCooler', icon: '/Images/sıvısogutma.webp', imgClass: 'h-24' }
    ];

    const containerVariants = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: { staggerChildren: 0.1 }
        }
    };

    const itemVariants = {
        hidden: { opacity: 0, y: 20 },
        visible: { opacity: 1, y: 0, transition: { duration: 0.5 } }
    };

    return (
        <div className="relative min-h-screen py-12 md:py-20 overflow-hidden px-4 sm:px-6 lg:px-12 xl:px-24">
            <main className="w-full">
                
                {/* Header Section */}
                <div className="text-center max-w-4xl mx-auto mb-12 md:mb-16">
                    <motion.h1 
                        initial={{ opacity: 0, y: -20 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="text-4xl md:text-5xl font-bold text-gray-900 dark:text-white tracking-tight"
                    >
                        {t["pbg-title"] || "PC Bileşenleri Rehberi"}
                    </motion.h1>
                    <motion.p 
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ delay: 0.2 }}
                        className="mt-4 text-lg text-gray-600 dark:text-gray-400"
                    >
                        {t["pbg-subtitle"] || "Her bir parçanın temel görevini ve sisteminizdeki önemini keşfedin."}
                    </motion.p>
                </div>

                {/* Categories Grid - 1:1 Old Style Remastered for Full Width */}
                <motion.div 
                    variants={containerVariants}
                    initial="hidden"
                    animate="visible"
                    className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
                >
                    {categories.map((cat) => (
                        <motion.div
                            key={cat.id}
                            variants={itemVariants}
                            whileHover={{ y: -5 }}
                            className="bg-card-light dark:bg-card-dark p-6 rounded-xl border border-gray-200 dark:border-gray-700 transition-all shadow-md hover:shadow-xl group"
                        >
                            <div className="component-placeholder bg-gray-50 dark:bg-gray-800/50 rounded-lg flex items-center justify-center mb-6 overflow-hidden h-48">
                                <img 
                                    src={cat.icon} 
                                    alt={cat.id} 
                                    className={`w-auto ${cat.imgClass} object-contain transition-transform duration-500 group-hover:scale-110`}
                                />
                            </div>
                            
                            <h3 className="text-xl font-bold text-gray-900 dark:text-white mb-3 group-hover:text-primary transition-colors">
                                {t[`pbg-cat-${cat.id}-title`] || t[`tab-${cat.id === 'motherboard' ? 'mobo' : cat.id === 'cpuCooler' ? 'cooler' : cat.id}`] || cat.id.toUpperCase()}
                            </h3>
                            
                            <p className="text-sm text-gray-600 dark:text-gray-400 leading-relaxed font-medium">
                                {t[`pbg-cat-${cat.id}-desc`] || "Bu bileşen hakkında detaylı teknik inceleme ve seçim rehberi yakında eklenecek."}
                            </p>
                            
                            <div className="mt-6 flex items-center gap-2 text-primary font-bold text-[10px] uppercase tracking-widest opacity-0 group-hover:opacity-100 transition-all">
                                <span>{lang === 'tr' ? 'DETAYLARI İNCELE' : 'VIEW DETAILS'}</span>
                                <span className="material-symbols-outlined text-sm">arrow_forward</span>
                            </div>
                        </motion.div>
                    ))}
                </motion.div>

                {/* Future Roadmap Section - Subtle Design */}
                <motion.div 
                    initial={{ opacity: 0 }}
                    whileInView={{ opacity: 1 }}
                    viewport={{ once: true }}
                    className="mt-24 p-12 rounded-3xl border border-dashed border-gray-200 dark:border-gray-800 bg-gray-50/30 dark:bg-white/[0.01] text-center"
                >
                    <p className="text-gray-400 dark:text-white/20 text-xs font-black uppercase tracking-[0.4em] mb-4">PCPartsBuild Roadmap</p>
                    <h3 className="text-2xl font-bold text-gray-500 dark:text-white/40 italic tracking-tight">
                        Deep Dive Technical Specifications & Performance Benchmarks Coming Soon
                    </h3>
                </motion.div>
            </main>
        </div>
    );
}
