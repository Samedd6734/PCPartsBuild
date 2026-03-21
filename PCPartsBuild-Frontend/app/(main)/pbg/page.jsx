'use client';

import React, { useEffect, useState } from 'react';
import { translations } from '@/lib/translations';

export default function PbgPage() {
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

    const t = translations[lang];

    const components = [
        { id: 'cpu', icon: '/Images/cpu.webp', title: t['cpu-title'], desc: t['cpu-desc'], imgClass: 'w-auto h-40' },
        { id: 'gpu', icon: '/Images/ekrankarti.webp', title: t['gpu-title'], desc: t['gpu-desc'], imgClass: 'w-auto h-32' },
        { id: 'ram', icon: '/Images/ram.webp', title: t['ram-title'], desc: t['ram-desc'], imgClass: 'w-auto h-32' },
        { id: 'ssd', icon: '/Images/ssd.webp', title: t['ssd-title'], desc: t['ssd-desc'], imgClass: 'w-60 h-32' },
        { id: 'mobo', icon: '/Images/anakart.webp', title: t['mobo-title'], desc: t['mobo-desc'], imgClass: 'w-auto h-40' },
        { id: 'psu', icon: '/Images/guc.webp', title: t['psu-title'], desc: t['psu-desc'], imgClass: 'w-auto h-40' },
        { id: 'case', icon: '/Images/kasa.webp', title: t['case-title'], desc: t['case-desc'], imgClass: 'w-auto h-40' },
        { id: 'cooler', icon: '/Images/sıvısogutma.webp', title: t['cooler-title'], desc: t['cooler-desc'], imgClass: 'w-auto h-24' }
    ];

    return (
        <main className="flex-grow container mx-auto px-4 py-12 md:py-20">
            <div className="text-center max-w-4xl mx-auto mb-12 md:mb-16">
                <h1 className="text-4xl md:text-5xl font-bold text-gray-900 dark:text-white tracking-tight">
                    {t['pbg-title']}
                </h1>
                <p className="mt-4 text-lg text-gray-600 dark:text-gray-400">
                    {t['pbg-subtitle']}
                </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                {components.map((comp) => (
                    <div 
                        key={comp.id}
                        className="bg-card-light dark:bg-card-dark p-6 rounded-lg border border-gray-200 dark:border-gray-700 transition-default shadow-md hover:shadow-lg hover:scale-[1.02]"
                    >
                        <div className="component-placeholder">
                            <img alt={`${comp.id} image`} className={comp.imgClass} src={comp.icon} />
                        </div>
                        <h3 className="text-xl font-semibold text-gray-900 dark:text-white mb-2">
                            {comp.title}
                        </h3>
                        <p className="text-sm text-gray-600 dark:text-gray-400">
                            {comp.desc}
                        </p>
                    </div>
                ))}
            </div>
        </main>
    );
}
